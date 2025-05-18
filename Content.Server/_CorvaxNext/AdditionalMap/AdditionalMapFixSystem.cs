// SPDX-FileCopyrightText: 2025 Kill_Me_I_Noobs <118206719+Vonsant@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Threading;
using Content.Server.Atmos.Components;
using Content.Server.GameTicking;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Events;
using Robust.Server.Console;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Server._CorvaxNext.AdditionalMapFix;
public sealed class AdditionalMapFix : EntitySystem
{
    [Dependency] private readonly IServerConsoleHost _host = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StationPostInitEvent>(OnStartup, after: new[] { typeof(ShuttleSystem) });
    }

    private void OnStartup(ref StationPostInitEvent args)
    {
        Timer.Spawn(TimeSpan.FromSeconds(5), () =>
        {
            var query = AllEntityQuery<GridAtmosphereComponent, TransformComponent>();

            while (query.MoveNext(out var dummyatmos, out var comp))
            {
                var gridUid = comp.GridUid;
                _host.AppendCommand($"fixgridatmos {gridUid}");
                Logger.Error($"executed command on {gridUid}");
            }
        });
    }
}
