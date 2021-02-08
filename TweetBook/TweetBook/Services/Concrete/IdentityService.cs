using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Data;
using TweetBook.Domain;
using TweetBook.Options;

namespace TweetBook.Services
{
	public class IdentityService : IIdentityService
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly JwtSettings _jwtSettings;
		private readonly TokenValidationParameters _tokenValidationParameters;
		private readonly DataContext _dataContext;

		public IdentityService(UserManager<IdentityUser> userManager, DataContext dataContext, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters)
		{
			this._userManager = userManager;
			this._jwtSettings = jwtSettings;
			this._tokenValidationParameters = tokenValidationParameters;
			this._dataContext = dataContext;
		}

		public async Task<AuthenticationResult> RegisterAsync(string email, string password)
		{
			var existingUser = await _userManager.FindByEmailAsync(email);
			if (existingUser != null)
			{
				return new AuthenticationResult
				{
					Errors = new[] { "User with this email address already exists!" }
				};
			}

			var newUserId = Guid.NewGuid();
			var newUser = new IdentityUser
			{
				Id = newUserId.ToString(),
				Email = email,
				UserName = email
			};

			var createdUser = await _userManager.CreateAsync(newUser, password);
			if (!createdUser.Succeeded)
			{
				return new AuthenticationResult
				{
					Errors = createdUser.Errors.Select(x => x.Description)
				};
			}

			await _userManager.AddClaimAsync(newUser, new Claim(Policy.Claims.TagsView, "true"));
			return await GenerateAuthenticationResultForUserAsync(newUser);
		}

		public async Task<AuthenticationResult> LoginAsync(string email, string password)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return new AuthenticationResult
				{
					Errors = new[] { "User does not exists!" }
				};
			}

			var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);
			if (!userHasValidPassword)
			{
				return new AuthenticationResult
				{
					Errors = new[] { "User/password combination is wrong!" }
				};
			}

			return await GenerateAuthenticationResultForUserAsync(user);

		}

		public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
		{
			var validatedToken = GetPrincipalFromToken(token);

			if (validatedToken == null)
				return new AuthenticationResult
				{ Errors = new[] { "Invalid token error!" } };

			var expiryDate = validatedToken.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Exp).Value;
			var expiryDateUnix = long.Parse(expiryDate);
			var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
				.AddSeconds(expiryDateUnix);

			if (expiryDateTimeUtc > DateTime.UtcNow)
				return new AuthenticationResult
				{ Errors = new[] { "This token hasn't expired yet!" } };


			var jti = validatedToken.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Jti).Value;
			var storedRefreshToken = await _dataContext.RefreshTokens.SingleOrDefaultAsync(token => token.Token == refreshToken);

			if (storedRefreshToken == null)
				return new AuthenticationResult
				{ Errors = new[] { "This token hasn't exists!" } };

			if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
				return new AuthenticationResult
				{ Errors = new[] { "This refresh token has expired!" } };

			if (storedRefreshToken.Invalidated)
				return new AuthenticationResult
				{ Errors = new[] { "This refresh token has been invalidated!" } };

			if (storedRefreshToken.IsUsed)
				return new AuthenticationResult
				{ Errors = new[] { "This refresh token has been used!" } };

			if (storedRefreshToken.JwtId != jti)
				return new AuthenticationResult
				{ Errors = new[] { "This refresh token does not match this JWT!" } };

			storedRefreshToken.IsUsed = true;
			_dataContext.RefreshTokens.Update(storedRefreshToken);
			await _dataContext.SaveChangesAsync();

			var user = await _userManager.FindByIdAsync(
				validatedToken.Claims
				.Single(claim => claim.Type == "id")
				.Value);

			return await GenerateAuthenticationResultForUserAsync(user);

		}

		private ClaimsPrincipal GetPrincipalFromToken(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();

			try
			{
				var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

				if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
					return null;

				return principal;
			}
			catch
			{
				return null;
			}

		}

		private bool IsJwtWithValidSecurityAlgorithm(SecurityToken token)
		{
			return (token is JwtSecurityToken jwtSecurityToken)
				&& jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature,
					StringComparison.InvariantCultureIgnoreCase);
		}

		private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim("id", user.Id),
			};

			var userClaims = await _userManager.GetClaimsAsync(user);
			claims.AddRange(userClaims);

			var tokenDescriptor = new SecurityTokenDescriptor()
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			var refreshToken = new RefreshToken
			{
				JwtId = token.Id,
				UserId = user.Id,
				CreationDate = DateTime.UtcNow,
				ExpiryDate = DateTime.UtcNow.AddMonths(6)
			};

			await _dataContext.RefreshTokens.AddAsync(refreshToken);
			await _dataContext.SaveChangesAsync();

			return new AuthenticationResult
			{
				Success = true,
				Token = tokenHandler.WriteToken(token),
				RefreshToken = refreshToken.Token
			};
		}
	}
}
