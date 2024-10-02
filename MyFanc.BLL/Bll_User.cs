using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFanc.BusinessObjects;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.Forms;
using MyFanc.DTO.Internal.OperationalEntity;
using MyFanc.DTO.Internal.Persona;
using MyFanc.DTO.Internal.Translation;
using MyFanc.DTO.Internal.Users;
using static MyFanc.Core.Enums;

namespace MyFanc.BLL
{
	public partial class Bll
	{
		public async Task<ProfileDTO> UpdateUserAsync(string id, UpdateProfileDTO user)
		{
			ProfileDTO result = new();
			var userRequest = new GetUserRequest()
			{
				UserId = id,
			};
			var userFromAPI = await _fancRADApi.GetUser(userRequest);
			UpdateUserInfoRequest updateUserInfoRequest = _mapper.Map<UpdateUserInfoRequest>(user);
			if (userFromAPI != null)
			{
				var updateUserRequest = new UpdateUserRequest()
				{
					UserId = id
				};
				await _fancRADApi.UpdateUser(updateUserInfoRequest, updateUserRequest);
				userFromAPI = await _fancRADApi.GetUser(userRequest);
				if (userFromAPI != null)
				{
					_mapper.Map(userFromAPI, result);
					return result;
				}
				else
				{
					throw new KeyNotFoundException($"Error on getting user information from fanc API {id}. {Services.Constants.UserNotFound}");
				}

			}
			else
			{
				throw new KeyNotFoundException($"Error on getting user information from fanc API {id}. {Services.Constants.UserNotFound}");

			}
		}

		public async Task<ProfileDTO> GetUserInfoAsync(GetUserRequest userRequest)
		{
			if (!string.IsNullOrWhiteSpace(userRequest?.UserId))
			{
				if (userRequest.ForceUpdate == true)
				{
					var userFromRepo = _userRepository.Find(u => u.ExternalId == userRequest.UserId && !u.DeletedTime.HasValue).FirstOrDefault();
					if (userFromRepo?.LatestSynchronization.Date == DateTime.Today)
					{
						throw new InvalidOperationException($"Cannot Get User Info {userRequest.UserId}. {Services.Constants.SynchronizationAlreadyDone}");
					}
					//as suggested if synchcronize user data, then clear cache data
					_sharedDataCache.ClearAllData();
				}

				GetUserInfoResult getUserInfoResult = await _fancRADApi.GetUser(userRequest);
				if (getUserInfoResult != null)
				{
					var userFromDb = _userRepository.Find(x => x.ExternalId == userRequest.UserId && !x.DeletedTime.HasValue).FirstOrDefault();

					if (userFromDb == null)
					{
						var user = _mapper.Map<User>(getUserInfoResult);
						user.ExternalId = userRequest.UserId;
						_userRepository.Add(user);
						await _unitOfWork.SaveChangesAsync();
					}
					else
					{
						_mapper.Map(getUserInfoResult, userFromDb);
						_userRepository.Update(userFromDb);
						await _unitOfWork.SaveChangesAsync();
					}

					ProfileDTO result = _mapper.Map<ProfileDTO>(getUserInfoResult);
					return result;
				}
				else
				{
					throw new KeyNotFoundException($"Cannot Get User Info {userRequest.UserId}. {Services.Constants.UserNotFound}");
				}
			}
			else
			{
				throw new ArgumentException($"Parameter is invalid (Value was: {userRequest?.UserId}). {Services.Constants.UserRequest}", nameof(userRequest));
			}
		}

		public async Task<bool> ForceEmailVerificationAsync(string userId)
		{
			if (string.IsNullOrEmpty(userId))
				throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

			var userRequest = new GetUserRequest()
			{
				UserId = userId
			};
			await _fancRADApi.VerifyEmail(userRequest);
			return true;
		}

		public async Task<AuthRedirectionDTO> GetAuthRedirectionAsync(string id, string? idp)
		{
			AuthRedirectionDTO result = new();
			var userFromRADApi = await _fancRADApi.GetUser(new GetUserRequest()
			{
				UserId = id,
			});

			if (userFromRADApi != null && !string.IsNullOrEmpty(id))
			{
				var userFromDb = _userRepository.Find(x => x.ExternalId == id && !x.DeletedTime.HasValue).FirstOrDefault();

				if (userFromDb == null)
				{
					var user = _mapper.Map<User>(userFromRADApi);
					user.ExternalId = id;
					_userRepository.Add(user);
					var savedEntries = await _unitOfWork.SaveChangesAsync();
					if (savedEntries <= 0)
					{
						_logger.LogError("The insert of the User has failed for an unknown reason {data}", user);
						throw new Exception("no change has been commited, User not written");
					}
				}
				else
				{
					_mapper.Map(userFromRADApi, userFromDb);
					_userRepository.Update(userFromDb);
					await _unitOfWork.SaveChangesAsync();
				}

				result.RedirecTo = (idp == _identityProviderConfiguration.ADFS) ? 
					AuthRedirection.WelcomePage 
					: GetApproriateRedirection(userFromRADApi);

				return result;
			}
			else
			{
				throw new KeyNotFoundException($"Error on getting user information from fanc-radapi API UserId = {id}");
			}
		}

		private static AuthRedirection GetApproriateRedirection(GetUserInfoResult user)
		{


			if (!IsAllMandatoryFieldsFilled(user))
			{
				return AuthRedirection.ProfilePage;
			}
			else if (IsValidated(user))
			{
				return AuthRedirection.WelcomePage;
			}
			else
			{
				return AuthRedirection.WaitingValidationPage;
			}
		}

		private static bool IsValidated(GetUserInfoResult user)
		{

			//this will check user validation
			//need technical flow for user validation, what to validate? and how to validate? 
			//for unit test converage purpose temporary will use NrNumber,
			//if NrNumber not null then validated otherwise is not validated
			//bool result = !string.IsNullOrEmpty(user.NrNumber);
			return user.UserIsValidated;
		}

		private static bool IsAllMandatoryFieldsFilled(GetUserInfoResult user)
		{
			//this will check all fields mandatory is filled
			if (string.IsNullOrEmpty(user.FirstName))
			{
				return false;
			}
			if (string.IsNullOrEmpty(user.LastName))
			{
				return false;
			}
			if (string.IsNullOrEmpty(user.Email))
			{
				return false;
			}
			if (string.IsNullOrEmpty(user.StructuredAddress?.CountryCode))
			{
				return false;
			}
			if (string.IsNullOrEmpty(user.StructuredAddress?.CityName))
			{
				return false;
			}
			if (string.IsNullOrEmpty(user.StructuredAddress?.StreetName))
			{
				return false;
			}
			if (string.IsNullOrEmpty(user.StructuredAddress?.HouseNumber))
			{
				return false;
			}
			if (string.IsNullOrEmpty(user.StructuredAddress?.PostalCode))
			{
				return false;
			}
			return true;
		}

		public async Task<UserIdentityDTO> GetUserIdentityAsync(GetUserRequest userRequest, string? idp)
		{
			GetUserInfoResult getUserInfoResult = await _fancRADApi.GetUser(userRequest);
			if (getUserInfoResult != null)
			{
				UserIdentityDTO result = _mapper.Map<UserIdentityDTO>(getUserInfoResult);
				if (!string.IsNullOrEmpty(idp))
					result.GlobalRoles.Add(idp == _identityProviderConfiguration.CSAM ? new OrganisationRoleDTO() { Role = Core.Constants.GlobalRolesBasicUser, LinkSource = _identityProviderConfiguration.CSAM } : new OrganisationRoleDTO() { Role = Core.Constants.GlobalRolesAdmin, LinkSource = _identityProviderConfiguration.ADFS });

				var userFromDb = _userRepository.Find(x => x.ExternalId == userRequest.UserId && !x.DeletedTime.HasValue).FirstOrDefault();

				var userOrganisationLinks = await _fancRADApi.GetUserOrganisationLinks(new GetUserOrganisationLinksRequest()
				{
					UserId = userRequest.UserId,
				});

				result.UserOrganisations = _mapper.Map<List<UserOrganisationsDTO>>(userOrganisationLinks.OrganisationLinks);
				FixUserRole(result.UserOrganisations);

				if (result.UserOrganisations.Any())
				{
					result.CurrentOrganisation = userFromDb?.LatestOrganisation ?? result.UserOrganisations.FirstOrDefault()?.Id ?? Guid.Empty;
					result.CurrentEstablishment = userFromDb?.LatestEstablishment ?? result.UserOrganisations.FirstOrDefault()?.Establishment?.FirstOrDefault()?.Id ?? Guid.Empty;
				}
				return result;
			}
			else
			{
				throw new KeyNotFoundException($"Cannot Get User Identity Info {userRequest.UserId}. {Services.Constants.UserNotFound}");
			}
		}

		private void FixUserRole(List<UserOrganisationsDTO> userOrganisationsDTO)
		{
			foreach (var le in userOrganisationsDTO)
			{
				le.Roles.RemoveAll(r => r.Role.ToLower().Contains("collaborator"));
				foreach (var oe in le.Establishment)
				{
					oe.Roles.RemoveAll(r => r.Role.ToLower().Contains("collaborator"));
					if (oe.Roles.FirstOrDefault(r => r.Role.ToLower().Contains("manager")) != null)
					{
						le.Roles.RemoveAll(r => !r.Role.ToLower().Contains("manager"));
					}
				}
			}
		}
		public async Task<string> AcceptInvitation(string userId, UpdateOrganisationLinkInfoRequest userRequest)
		{
			try
			{
				var user = new UpdateUserRequest()
				{
					UserId = userId
				};
				// TODO : accept invitation handled by rad api
				await _fancRADApi.UpdateUserOrganisationLinks(userRequest, user);
				_logger.LogInformation("Success accept invitation for user {0}", userId);

				// TODO : send email here, not implemented yet
				var invitationCreator = await GetUserInfoAsync(new GetUserRequest()
				{
					UserId = userRequest.RequestingUserId
				});
				if (invitationCreator != null && !string.IsNullOrEmpty(invitationCreator.Email))
				{
					await _emailService.SendInvitationAcceptOrRefuseNotificationAsync(invitationCreator.Email, userId, userRequest.FancOrganisationId, userRequest.Roles, true);
				}

				return "You successfully accepted to join the Operational Entity. The manager of the entity will be informed.";
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Accepting invitation is fail {0}", ex.Message);
				throw new Exception("Something went wrong, invitation failed to accept for some reason!");
			}
		}

		public async Task<string> RefuseInvitation(string userId, UpdateOrganisationLinkInfoRequest userRequest)
		{
			try
			{
				var user = new UpdateUserRequest()
				{
					UserId = userId
				};
				// TODO : refuse invitation handled by rad api
				await _fancRADApi.DeleteUserOrganisationLinks(userRequest, user);
				_logger.LogInformation("Success delete invitation for user {0}", userId);

				// TODO : send email here, not implemented yet
				var invitationCreator = await GetUserInfoAsync(new GetUserRequest()
				{
					UserId = userRequest.RequestingUserId
				});
				if (invitationCreator != null && !string.IsNullOrEmpty(invitationCreator.Email))
				{
					await _emailService.SendInvitationAcceptOrRefuseNotificationAsync(invitationCreator.Email, userId, userRequest.FancOrganisationId, userRequest.Roles, false);
				}

				return "You successfully refused to join the Operational Entity. The manager of the entity will be informed.";
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Refuse invitation is fail {0}", ex.Message);
				throw new Exception("Something went wrong, invitation failed to delete for some reason!");
			}
		}

		private async Task<List<OrganisationUsersDTO>> GetUserOrganisation(string userId)
		{
			string cacheKey = $"User-{userId}-Organisations";
			var cacheCountryInfo = _sharedDataCache.GetData<List<OrganisationUsersDTO>>(cacheKey);
			if (cacheCountryInfo?.Any() != true) await StoreUserOrganisation(userId);
			cacheCountryInfo = _sharedDataCache.GetData<List<OrganisationUsersDTO>>(cacheKey);
			return cacheCountryInfo;
		}

		public async Task<List<string>> GetUserRoles(GetUserOrganisationLinksRequest userRequest)
		{
			if (!string.IsNullOrWhiteSpace(userRequest?.UserId))
			{
				// TODO : Initialization
				var result = new List<string>();
				var userRolesFromRad = new List<string>();

				// TODO : get user roles from organisation links
				var userOrganisations = await _fancRADApi.GetUserOrganisationLinks(userRequest);

				if (userOrganisations != null && userOrganisations.OrganisationLinks.Any())
				{
					foreach (var item in userOrganisations.OrganisationLinks)
					{
						var roles = item.Roles.Select(x => x.Role).ToList();
						if (roles.Count > 0)
							userRolesFromRad.AddRange(roles);
					}
				}
				else
				{
					_logger.LogInformation("Error on getting user information from fanc API {0} {1}", userRequest.UserId, Services.Constants.UserOrganitationLinkNotFound);
					throw new KeyNotFoundException($"Error on getting user information from fanc API {userRequest.UserId}. {Services.Constants.UserOrganitationLinkNotFound}");
				}

				// TODO : get user from db by external id
				var userFromDb = _userRepository.Find(x => x.ExternalId == userRequest.UserId && !x.DeletedTime.HasValue)
					.Include(x => x.UserRoles)
					.AsSplitQuery()
					.FirstOrDefault();

				if (userFromDb == null)
				{
					_logger.LogInformation("Cannot Get User Identity Info {0} {1}", userRequest.UserId, Services.Constants.UserNotFound);
					throw new KeyNotFoundException($"Cannot Get User Identity Info {userRequest}. {Services.Constants.UserNotFound}");
				}
				// TODO : get roles
				var rolesFromDb = _rolesRepository.Find(x => !x.DeletedTime.HasValue).ToList();

				// TODO : mapping userorganisation roles to internal roles
				foreach (var role in rolesFromDb)
					if (userRolesFromRad.Contains(role.ExternalRole)) result.Add(role.InternalRole);

				// TODO : add default UserRoles to result id exist
				var userRoles = userFromDb?.UserRoles.Select(x => x.InternalRole).ToList();
				if (userRoles != null)
					result.AddRange(userRoles);
				return result;
			}
			_logger.LogError("Parameter is invalid (Value was: {0}). {1}", userRequest?.UserId, Services.Constants.UserRequest);
			throw new ArgumentException($"Parameter is invalid (Value was: {userRequest?.UserId}). {Services.Constants.UserRequest}", nameof(userRequest));
		}
		public async Task<SelectorUserDTO> UpdateCurrentSelector(string userId, SelectorUserDTO userRequest)
		{
			var userFromDb = _userRepository.Find(x => x.ExternalId == userId && !x.DeletedTime.HasValue).FirstOrDefault();
			if (userFromDb == null) throw new KeyNotFoundException($"Invalid user ID. {userId}");
			_mapper.Map(userRequest, userFromDb);
			_userRepository.Update(userFromDb);
			var savedEntries = await _unitOfWork.SaveChangesAsync();
			if (savedEntries <= 0)
			{
				_logger.LogError("The update of the User has failed for an unknown reason {data}", userFromDb);
				throw new Exception("no change has been commited, User not updated");
			}
			var map = _mapper.Map<SelectorUserDTO>(userFromDb);
			return map;
		}

		private void GetUserPersona(PersonaDTO result, string userId, string languageCode = "en")
		{
			var userFromRepo = _userRepository.Find(u => u.ExternalId == userId && !u.DeletedTime.HasValue).FirstOrDefault() ?? throw new KeyNotFoundException($"Invalid user ID. {userId}");
			
			var user = _userPersonasRepository.Find(u => u.UserId == userFromRepo.Id && !u.DeletedTime.HasValue).FirstOrDefault();
			if (user != null)
			{
				var userPersona = _userPersonaCategoriesRepository.Find(x => x.UserPersonaId == user.Id);
				var personaCategories = _personaCategoriesRepository.Find(x => userPersona.Any(y => y.PersonaCategoryId == x.Id))
										.AsSplitQuery()
										.Include(x => x.Labels.Where(y => y.LanguageCode == languageCode))
										.Include(x => x.Nacebel).ThenInclude(n => n.NacabelTranslation.Where(y => y.LanguageCode == languageCode))
										.ToList();
				result.UserPersona = _mapper.Map<UserPersonaDTO>(user);

				var mapperCategory = _mapper.Map<List<CategoryDTO>>(personaCategories);
				result.UserPersona.Categories = mapperCategory;
			}
		}

		private void GetCompanyPersona(PersonaDTO result, Guid OrganisationFancId, string languageCode = "en")
		{
			var company = _companyPersonasRepository.Find(u => u.OrganisationFancId == OrganisationFancId && !u.DeletedTime.HasValue).FirstOrDefault();
			if (company != null)
			{
				var companyPersona = _companyPersonaCategoriesRepository.Find(x => x.CompanyPersonaId == company.Id);
				var personaCategories = _personaCategoriesRepository.Find(x => companyPersona.Any(y => y.PersonaCategoryId == x.Id))
										.AsSplitQuery()
										.Include(x => x.Labels.Where(y => y.LanguageCode == languageCode))
										.Include(x => x.Nacebel).ThenInclude(n => n.NacabelTranslation.Where(y => y.LanguageCode == languageCode))
										.ToList();
				result.CompanyPersona = _mapper.Map<CompanyPersonaDTO>(company);
				var mapperCategory = _mapper.Map<List<CategoryDTO>>(personaCategories);
				result.CompanyPersona.Categories = mapperCategory;
			}
		}
		public PersonaDTO GetUserCompanyPersona(string userId, Guid OrganisationFancId, string languageCode = "en")
		{
			try
			{
				var result = new PersonaDTO();
				GetUserPersona(result, userId, languageCode);
				GetCompanyPersona(result, OrganisationFancId, languageCode);
				return result;
			}
			catch (Exception)
			{
				throw new InvalidOperationException("Failed to retrieve the user persona");
			}
		}

		public async Task<string> SaveUserAndCompanyPersona(string userId, AddOrUpdatePersonaDTO param)
		{
			var userFromRepo = _userRepository.Find(u => u.ExternalId == userId && !u.DeletedTime.HasValue).FirstOrDefault() ?? throw new KeyNotFoundException($"Invalid user ID. {userId}");
			if (param.UserPersona == null) throw new KeyNotFoundException("No User to update");
			await SaveOrUpdateUserPersona(userFromRepo.Id, param.UserPersona);

			if (param.CompanyPersona != null)
			{
				await SaveOrUpdateCompanyPersona(param.CompanyPersona);
			}
			return "Success";
		}

		private async Task<string> SaveOrUpdateUserPersona(int userId, AddOrUpdateUserDTO param)
		{
			try
			{
				var userPersonaDb = await _userPersonasRepository.Find(x => x.UserId == userId && !x.DeletedTime.HasValue).FirstOrDefaultAsync();
				if (userPersonaDb == null)
				{
					try
					{
						_unitOfWork.BeginTransaction();
						// TODO : create
						AddUserPersonaDTO newUser = new AddUserPersonaDTO
						{
							InamiNumber = param.InamiNumber,
							UserId = userId,
						};
						var userMapper = _mapper.Map<UserPersonas>(newUser);

						_userPersonasRepository.Add(userMapper);
						await _unitOfWork.SaveChangesAsync();

						foreach (var item in param.Categories)
						{
							var category = new AddUserPersonaCategoriesDTO();
							category.PersonaCategoryId = item;
							category.UserPersonaId = userMapper.Id;
							var userCategoryMapper = _mapper.Map<UserPersonaCategories>(category);
							_userPersonaCategoriesRepository.Add(userCategoryMapper);
						}
						await _unitOfWork.SaveChangesAsync();

						_unitOfWork.CommitTransaction();
					}
					catch (Exception)
					{
						_unitOfWork.RollbackTransaction();
						throw;
					}
				}
				else
				{
					try
					{
						_unitOfWork.BeginTransaction();
						// Update user persona only inami number
						userPersonaDb.InamiNumber = param.InamiNumber;
						// Update userpersonacategories
						ApplyChangeOnUserPersonaCategories(param.Categories, userPersonaDb);

						await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
						_unitOfWork.CommitTransaction();
					}
					catch (Exception)
					{
						_unitOfWork.RollbackTransaction();
						throw;
					}
				}
				return "Success";
			}
			catch (Exception)
			{
				_unitOfWork.RollbackTransaction();
				throw;
			}
		}

		private void ApplyChangeOnUserPersonaCategories(IEnumerable<int> colPersonaCategoriesDto, UserPersonas colUserPersonaEntity)
		{
			var userPersonaCategoriesDb = _userPersonaCategoriesRepository.Find(x => x.UserPersonaId == colUserPersonaEntity.Id).ToList();
			var toDelete = userPersonaCategoriesDb.Where(t => !colPersonaCategoriesDto.Any(s => s == t.PersonaCategoryId)).ToList();
			foreach (var item in toDelete.ToList())
			{
				userPersonaCategoriesDb.Remove(item);
				_userPersonaCategoriesRepository.Delete(item);
			}
			foreach (var item in colPersonaCategoriesDto)
			{
				var existing = userPersonaCategoriesDb.FirstOrDefault(t => t.PersonaCategoryId == item && t.UserPersonaId == colUserPersonaEntity.Id);
				// if not exist then add
				if (existing == null)
				{
					var category = new AddUserPersonaCategoriesDTO();
					category.PersonaCategoryId = item;
					category.UserPersonaId = colUserPersonaEntity.Id;
					var userCategoryMapper = _mapper.Map<UserPersonaCategories>(category);
					_userPersonaCategoriesRepository.Add(userCategoryMapper);
				}
			}
		}

		private async Task<string> SaveOrUpdateCompanyPersona(AddOrUpdateCompanyDTO param)
		{
			var companyPersonaDb = await _companyPersonasRepository.Find(x => x.OrganisationFancId == param.OrganisationId && !x.DeletedTime.HasValue).FirstOrDefaultAsync();
			if (companyPersonaDb == null)
			{
				try
				{
					_unitOfWork.BeginTransaction();
					AddCompanyPersonaDTO newCompany = new AddCompanyPersonaDTO
					{
						OrganisationFancId = param.OrganisationId,
						NacabelCode = param.NacabelCode
					};
					var companyPersonaMapper = _mapper.Map<CompanyPersonas>(newCompany);

					_companyPersonasRepository.Add(companyPersonaMapper);
					await _unitOfWork.SaveChangesAsync();

					foreach (var item in param.Categories)
					{
						var category = new AddCompanyPersonaCategoriesDTO();
						category.PersonaCategoryId = item;
						category.CompanyPersonaId = companyPersonaMapper.Id;
						var companyCategoryMapper = _mapper.Map<CompanyPersonaCategories>(category);
						_companyPersonaCategoriesRepository.Add(companyCategoryMapper);
					}
					await _unitOfWork.SaveChangesAsync();

					_unitOfWork.CommitTransaction();
				}
				catch (Exception)
				{
					_unitOfWork.RollbackTransaction();
					throw;
				}
			}
			else
			{
				try
				{
					_unitOfWork.BeginTransaction();
					//update company persona only NacebelCode number
					companyPersonaDb.NacabelCode = param.NacabelCode;
					// update companypersonacategories
					ApplyChangeOnCompanyPersonaCategories(param.Categories, companyPersonaDb);

					await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
					_unitOfWork.CommitTransaction();
				}
				catch (Exception)
				{
					_unitOfWork.RollbackTransaction();

					throw;
				}
			}
			return "Success";
		}

		private void ApplyChangeOnCompanyPersonaCategories(IEnumerable<int> colPersonaCategoriesDto, CompanyPersonas colCompanyPersonaEntity)
		{
			var CompanyPersonaCategoriesDb = _companyPersonaCategoriesRepository.Find(x => x.CompanyPersonaId == colCompanyPersonaEntity.Id).ToList();
			var toDelete = CompanyPersonaCategoriesDb.Where(t => !colPersonaCategoriesDto.Any(s => s == t.PersonaCategoryId)).ToList();
			foreach (var item in toDelete.ToList())
			{
				CompanyPersonaCategoriesDb.Remove(item);
				_companyPersonaCategoriesRepository.Delete(item);
			}
			foreach (var item in colPersonaCategoriesDto)
			{
				var existing = CompanyPersonaCategoriesDb.FirstOrDefault(t => t.PersonaCategoryId == item && t.CompanyPersonaId == colCompanyPersonaEntity.Id);
				//If not exist then add
				if (existing == null)
				{
					var category = new AddCompanyPersonaCategoriesDTO();
					category.PersonaCategoryId = item;
					category.CompanyPersonaId = colCompanyPersonaEntity.Id;
					var CompanyCategoryMapper = _mapper.Map<CompanyPersonaCategories>(category);
					_companyPersonaCategoriesRepository.Add(CompanyCategoryMapper);
				}
			}
		}
	}
}
