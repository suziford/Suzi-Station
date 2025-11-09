// SPDX-FileCopyrightText: 2025 Hero010h <163765999+Hero010h@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared.Body.Components;
using Content.Shared.EntityEffects;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using System.Linq;

namespace Content.Server.EntityEffects.Effects;

public sealed partial class SpaceAdaptation : EntityEffect
{
    private readonly string _spaceHeartProto = "OrganSpaceAnimalHeart";
    private readonly string _spaceLungsProto = "OrganSpaceAnimalLungs";

    public override void Effect(EntityEffectBaseArgs args)
    {
        var entityManager = args.EntityManager;
        if (!entityManager.TryGetComponent<BodyComponent>(args.TargetEntity, out var body))
            return;

        var bodySystem = entityManager.System<BodySystem>();
        var containerSystem = entityManager.System<ContainerSystem>();
        var xFormSystem = entityManager.System<TransformSystem>();

        var organs = bodySystem.GetBodyOrgans(args.TargetEntity, body);

        foreach (var organ in organs)
        {
            if (entityManager.HasComponent<HeartComponent>(organ.Id))
            {
                ReplaceOrgan(organ.Id, _spaceHeartProto, entityManager, xFormSystem, containerSystem);
                continue;
            }

            if (entityManager.HasComponent<LungComponent>(organ.Id))
            {
                ReplaceOrgan(organ.Id, _spaceLungsProto, entityManager, xFormSystem, containerSystem);
                continue;
            }
        }

    }

    private static void ReplaceOrgan(EntityUid organ, string replaceWithProto,
        IEntityManager entityManager,
        TransformSystem xFormSystem,
        ContainerSystem containerSystem)
    {
        if (entityManager.GetComponent<MetaDataComponent>(organ).EntityPrototype is EntityPrototype organProto
            && organProto.ID == replaceWithProto)
            return;

        var xForm = entityManager.GetComponent<TransformComponent>(organ);
        var container = containerSystem.GetContainingContainers((organ, xForm)).First();

        var newOrgan = entityManager.Spawn(replaceWithProto);
        var newXForm = entityManager.GetComponent<TransformComponent>(newOrgan);
        var newMetaData = entityManager.GetComponent<MetaDataComponent>(newOrgan);
        var newPhysics = entityManager.GetComponent<PhysicsComponent>(newOrgan);

        xFormSystem.DetachEntity(organ, xForm);
        entityManager.QueueDeleteEntity(organ);

        containerSystem.Insert((newOrgan, newXForm, newMetaData, newPhysics), container);
    }

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString("reagent-effect-guidebook-space-adaptation");
    }
}
