﻿using AutoMapper;
using ChatApp.Business.DTO_s.Autheticate;
using ChatApp.Business.DTO_s.Errors;
using ChatApp.Business.Exceptions;
using ChatApp.Business.Helpers;
using ChatApp.Business.Services.Interfaces;
using ChatApp.Core;
using ChatApp.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace ChatApp.Business.Services.Implementations
{
    public class UserService : IUserService
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<RegisterResult> Register(Register register)
        {
            RegisterResult registerResult = new RegisterResult();
            User isEmail = await _userManager.FindByNameAsync(register.Email);
            if (isEmail != null) throw new AlreadyExistsException("Already exception");
            User user = _mapper.Map<User>(register);
            IdentityResult result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
            {
                registerResult.Error = result.Errors;
                return registerResult;
            };
            await _userManager.AddToRoleAsync(user, Roles.Member.ToString());
            return registerResult;
        }
        #region CreateRoles
        public async Task CreateRoles()
        {
            foreach (var item in Enum.GetValues(typeof(Roles)))
            {
                if (!(await _roleManager.RoleExistsAsync(item.ToString())))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = item.ToString()
                    });
                }
            }
        }
        #endregion
    }
}
