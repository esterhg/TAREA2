using CommunityToolkit.Mvvm.Messaging.Messages;

namespace TAREA2.Utilidades
{
    public class PersonaMensajeria : ValueChangedMessage<PersonaMensaje>
    {
        public PersonaMensajeria(PersonaMensaje value) : base(value)
        {

        }
    }
}
