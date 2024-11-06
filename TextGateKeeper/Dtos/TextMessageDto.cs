using System.ComponentModel.DataAnnotations.Schema;

namespace TextGateKeeper.Dtos {
    public partial class TextMessageDto {
        
        public string Body { get; set; }
        public string PhoneNumberFrom { get; set; }
        public string PhoneNumberTo { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public TextMessageDto () {
            if (Body == null) {
                Body = "";
            }

            if (PhoneNumberFrom == null) {
                PhoneNumberFrom = "";
            }

            if (PhoneNumberTo == null) {
                PhoneNumberTo = "";
            }
        }
    }
}