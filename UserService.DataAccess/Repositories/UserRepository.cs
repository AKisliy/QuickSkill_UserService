using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using UserService.Core.Interfaces;
using UserService.Core.Models;
using UserService.DataAccess.Entities;

namespace UserService.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private IMapper _mapper;
        private UserServiceContext _context;


        public UserRepository(UserServiceContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var userEntities = await _context.Users.AsNoTracking().ToListAsync();

            return userEntities.Select(u => _mapper.Map<User>(u));
        }

        public async Task Create(User user)
        {
            var find = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if(find != null)
                throw new InvalidDataException("User already registered");
            var userEntity = _mapper.Map<UserEntity>(user);
            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();
        }

        public async Task<int> Update(User user)
        {
            await _context.Users.Where(u => u.Id == user.Id).ExecuteUpdateAsync(s => s
                .SetProperty(u => u.Description, _ => user.Description)
                .SetProperty(u => u.Email, _ => user.Email)
                .SetProperty(u => u.FirstName, _ => user.FirstName)
                .SetProperty(u => u.GoalDays, _ => user.GoalDays)
                .SetProperty(u => u.GoalText, _ => user.GoalText)
                .SetProperty(u => u.LastName, _ => user.LastName)
                .SetProperty(u => u.Photo, _ => user.Photo)
                .SetProperty(u => u.UserLevel, _ => user.UserLevel)
                .SetProperty(u => u.Xp, _ => user.Xp)
                .SetProperty(u => u.Status, _ => user.Status));
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<int> Delete(User user)
        {
            await _context.Users.Where(u => u.Id == user.Id).ExecuteDeleteAsync();
            return user.Id;
        }

        public async Task<User?> GetUserById(int id)
        {
            var user =  await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(user == null)
                return null;
            return _mapper.Map<User>(user);
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            var user =  await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if(user == null)
                return null;
            return _mapper.Map<User>(user);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            var user =  await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if(user == null)
                return null;
            return _mapper.Map<User>(user);
        }
    }
}