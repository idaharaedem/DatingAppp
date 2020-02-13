using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppp.API.Helpers;
using DatingAppp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingAppp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
           _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            var mainUserPhoto = await _context.Photos.Where(p => p.UserId == userId).
            FirstOrDefaultAsync(u => u.IsMain);

            return mainUserPhoto;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<User> GetUser(int id)
        {
           var user = await _context.Users.Include( p => p.Photos).
            FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<PageList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive)
            .AsQueryable();

            users = users.Where( u => u.Id != userParams.UserId);

            users = users.Where(users=> users.Gender == userParams.Gender);

            if(userParams.Likers)
            {
                var userLikes = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikes.Contains(u.Id));
            }

            if(userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            if(userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy) 
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }
            return await PageList<User>.CreatAsync(users, userParams.PageNumber,userParams.PageSize);

            

        }

        private async Task <IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users.Include(l => l.Likers).Include(l => l.Likees)
            .FirstOrDefaultAsync(u => u.Id == id);

            if(likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }


        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
            .Include(m => m.Sender).ThenInclude(m => m.Photos).Include(r => r.Recipient)
            .ThenInclude(r => r.Photos)
            .Where(m => m.RecipientId == userId && m.RecipientDeleted== false && m.SenderId == recipientId || 
            m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted == false)
            .OrderByDescending(m => m.MessagesSent)
            .ToListAsync();

            return messages;
        }

        public async Task<PageList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
            .Include(m => m.Sender).ThenInclude(m => m.Photos).Include(r => r.Recipient)
            .ThenInclude(r => r.Photos).AsQueryable();

            switch(messageParams.MessageContainer)
            {
                case "Inbox": 
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
                    break; 
                
                case "Outbox":
                    messages = messages.Where(u => u.SenderId == messageParams.UserId && u.SenderDeleted == false);
                    break;
                
                default:
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false && u.IsRead == false);
                    break;
            }

            messages = messages.OrderByDescending( m => m.MessagesSent);

            return await PageList<Message>.CreatAsync(messages, 
            messageParams.PageNumber, messageParams.PageSize);
            
        }
    }
}