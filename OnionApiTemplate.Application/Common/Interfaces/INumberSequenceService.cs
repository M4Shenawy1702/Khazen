namespace Khazen.Application.Common.Interfaces
{
    public interface INumberSequenceService
    {
        Task<string> GetNextNumber(string prefix, int year, CancellationToken cancellationToken);
    }
}
