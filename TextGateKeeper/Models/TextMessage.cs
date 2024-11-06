using System.ComponentModel.DataAnnotations.Schema;

namespace TextGateKeeper.Models {
    public partial class TextMessage {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        public string Body { get; set; }
        public string PhoneNumberFrom { get; set; }
        public string PhoneNumberTo { get; set; }
        public DateTime CreatedDate { get; set; }

        public TextMessage () {
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