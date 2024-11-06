using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TextGateKeeper.Data;
using TextGateKeeper.Dtos;
using TextGateKeeper.Models;

namespace TextGateKeeper.Controllers {
    [ApiController]
    [Route("[Controller]")]
    public class SmsController: ControllerBase {
        IMapper _mapper;
        private readonly IConfiguration _config;
        ITextMessageRepository _TextMessageRepository;
        private int _maxLimitAccountPerSecond;
        private int _maxLimitPhonePerSecond;
        private int _cutOffInDays;
        
        public SmsController(IConfiguration config, ITextMessageRepository textMessageRepository){
            _config = config;
            _TextMessageRepository = textMessageRepository;
            _maxLimitAccountPerSecond = Convert.ToInt32(_config.GetSection("AppSettings:MaxLimitAccountPerSecond").Value);
            _maxLimitPhonePerSecond = Convert.ToInt32(_config.GetSection("AppSettings:MaxLimitPhonePerSecond").Value);
            _cutOffInDays = Convert.ToInt32(_config.GetSection("AppSettings:CutOffInDays").Value);
            _mapper = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<TextMessageDto, TextMessage>();
            }));
        }

        [HttpGet("Health")]
        public IActionResult Health() {
            return Ok("Healthy. It Works.");
        }
        
        [HttpPost("SendText")]
        public IActionResult SendText(TextMessageDto textMessage){
            // map
            TextMessage textMessageDb = _mapper.Map<TextMessage>(textMessage);
            
            // Get some counts
            int textMessageCountAccountLastSecond = _TextMessageRepository.GetTextMessageCountPerAccountFromLastSecond();
            int textMessageCountPhoneLastSecond = _TextMessageRepository
                .GetTextMessageCountPerPhoneFromLastSecond(textMessage.PhoneNumberFrom);
            
            // Run the gatekeeper rules
            if (textMessageCountAccountLastSecond > _maxLimitAccountPerSecond) {
                return BadRequest("Message Count exceeds maximum limit per Account from last second.");
            } else if (textMessageCountPhoneLastSecond > _maxLimitPhonePerSecond) {
                return BadRequest("Message Count exceeds maximum limit per Phone from last second.");
            } else {
                /*
                Put the code to send SMS. For example, Send SMS with Twilio. 

                string accountSid = "YOUR_ACCOUNT_SID";
                string authToken = "YOUR_AUTH_TOKEN";

                // Initialize the Twilio client
                TwilioClient.Init(accountSid, authToken);

                // Create and send the SMS message
                var message = MessageResource.Create(
                    body: "Hello from Twilio!",
                    from: new PhoneNumber("YOUR_TWILIO_PHONE_NUMBER"), // Your Twilio number
                    to: new PhoneNumber("RECIPIENT_PHONE_NUMBER") // The recipient's phone number
                );

                */

                // Log the Text Message in Database
                _TextMessageRepository.AddEntity<TextMessage>(textMessageDb); 

                if (_TextMessageRepository.SaveChanges()) {
                    return Ok("Success.");
                } else {
                    return BadRequest($"Unable to log {textMessageDb.PhoneNumberFrom} message");
                }
            }  
        }

        [HttpDelete("RemoveInActiveSms")]
        public IActionResult RemoveInActiveSms() {
            int result = _TextMessageRepository.RemoveInactiveTextMessage(_cutOffInDays);

            return Ok("Success.");
        }

    }
}