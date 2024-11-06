using TextGateKeeper.Models;

namespace TextGateKeeper.Data {
    public interface ITextMessageRepository{
        public bool SaveChanges();
        public void AddEntity<T>(T entityToAdd);
        public void RemoveEntity<T>(T entityToRemove);
        public IEnumerable<TextMessage> GetTextMessages();
        public TextMessage GetSingleTextMessage(int id);
        public int GetTextMessageCountPerPhoneFromLastSecond(string phoneNumFrom);
        public int GetTextMessageCountPerAccountFromLastSecond();
        public int RemoveInactiveTextMessage(int cutOffInDays);

    }
}