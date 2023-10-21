using CommunityToolkit.Mvvm.Messaging.Messages;
using SampleMauiMvvmApp.Models;

namespace SampleMauiMvvmApp.Messages
{
    public class CustomerCreateMessage : ValueChangedMessage<Customer>
    {
        public CustomerCreateMessage(Customer value) : base(value)
        {

        }
    }
}
