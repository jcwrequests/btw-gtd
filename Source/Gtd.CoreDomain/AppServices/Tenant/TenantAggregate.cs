﻿using System;
using System.Collections.Generic;

namespace Gtd.CoreDomain.AppServices.Tenant
{
    public sealed class TenantAggregate
    {
        readonly TenantState _state;

        public List<Event> EventsThatHappened = new List<Event>();

        public TenantAggregate(TenantState state)
        {
            _state = state;
        }

        public void Create(TenantId id)
        {
            Apply(new TenantCreated(id));
        }

        public void DefineProject(Guid requestId, string name, ITimeProvider provider)
        {
            // filter request IDs
            var time = provider.GetUtcNow();
            var id = new ProjectId(NewGuidIfEmpty(requestId));

            Apply(new ProjectDefined(_state.Id, id, name, time));
        }

        public void CaptureInboxEntry(Guid requestId, string name, ITimeProvider provider)
        {
            // filter request IDs
            var time = provider.GetUtcNow();
            var id = new ActionId(NewGuidIfEmpty(requestId));

            Apply(new InboxEntryCaptured(_state.Id, requestId, name));

            //Apply(new ActionCaptured(_state.Id, id, name, time));
        }

        static Guid NewGuidIfEmpty(Guid request)
        {
            return request == Guid.Empty ? Guid.NewGuid() : request;
        }

        void Apply(ITenantEvent e)
        {
            _state.MakeStateRealizeThat(e);
            EventsThatHappened.Add((Event) e);
        }

        public void DefineAction(Guid requestId, string actionName, ITimeProvider provider)
        {
            // filter request IDs
            var time = provider.GetUtcNow();
            var id = new ActionId(NewGuidIfEmpty(requestId));

            Apply(new ActionDefined(_state.Id, id, actionName, time));
        }
    }
}