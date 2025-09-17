using System;

namespace UBGKO.Party
{
    public static class PartyEvents
    {
        public static void RaiseOnPartyCreated() => OnPartyCreated?.Invoke();
        public static event Action OnPartyCreated;

        public static void RaiseNextInteractorRequest() => OnNextInteractorRequested?.Invoke();
        public static event Action OnNextInteractorRequested;

        public static void RaisePreviousInteractorRequest() => OnPreviousInteractorRequested?.Invoke();
        public static event Action OnPreviousInteractorRequested;

        public static void RaiseInteractorChanged(PartyMember member) => OnInteractorChanged?.Invoke(member);
        public static event Action<PartyMember> OnInteractorChanged;
    }
}