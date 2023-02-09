using static_sv.DTOs;

namespace static_sv.Interfaces
{
    public interface IRequestValidator {
        public Tuple<bool, string> Validate(object content, string signature);
    }
}