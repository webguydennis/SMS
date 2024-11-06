using TextGateKeeper.Models;

namespace TextGateKeeper.Data {
    public class TextMessageRepository : ITextMessageRepository {
        DataContextEF _entityFramework;
        
        public TextMessageRepository(IConfiguration config) {
            _entityFramework = new DataContextEF(config);
            
        }

        public bool SaveChanges(){
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd){
            if (entityToAdd != null) {
                _entityFramework.Add(entityToAdd);
            }
        }

        public void RemoveEntity<T>(T entityToRemove){
            if (entityToRemove != null) {
                _entityFramework.Remove(entityToRemove);
            }
        }

        public IEnumerable<TextMessage> GetTextMessages()
        { 
            IEnumerable<TextMessage> textMessages = _entityFramework.textMessages.ToList<TextMessage>();
            
            return textMessages;
        }
        
        public TextMessage GetSingleTextMessage(int id)
        { 
            TextMessage? textMessage = _entityFramework.textMessages
                .Where(t => t.Id == id)
                .FirstOrDefault<TextMessage>();

            if (textMessage != null) {
                return textMessage;
            }
            
            throw new Exception("Failed to get Text Message!");
        }

        public int GetTextMessageCountPerPhoneFromLastSecond(string phoneNumFrom){
            int textMessageCount = _entityFramework.textMessages
                .Where(t => t.PhoneNumberFrom == phoneNumFrom && t.CreatedDate >= DateTime.Now.AddSeconds(-1))
                .Count();
            
            return textMessageCount;
        }

        public int GetTextMessageCountPerAccountFromLastSecond() {
            int textMessageCount = _entityFramework.textMessages
                .Where(t => t.CreatedDate >= DateTime.Now.AddSeconds(-1))
                .Count();

            return textMessageCount;
        }

        public int RemoveInactiveTextMessage(int cutOffInDays) {
            DateTime cutOffDate = DateTime.Now.Date;
            cutOffDate = cutOffDate.AddDays(- cutOffInDays ).AddHours(23).AddMinutes(59).AddSeconds(59);

            // get which to remove
            IEnumerable<TextMessage> textMessagesToDelete = _entityFramework.textMessages
                .Where(t => t.CreatedDate <= cutOffDate);
            
            if (textMessagesToDelete.Any()) {
                // remove the selected text messages
                _entityFramework.RemoveRange(textMessagesToDelete);

                // returns how were removed
                return _entityFramework.SaveChanges();
            }

            return 0;
        }
    }
}