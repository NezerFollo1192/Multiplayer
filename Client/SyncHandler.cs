﻿using Harmony;
using Multiplayer.Common;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Verse;
using Verse.AI;
using MultiType = Verse.Pair<System.Type, string>;

namespace Multiplayer.Client
{
    public static class SyncFieldsPatches
    {
        public static SyncField SyncAreaRestriction = Sync.Field(typeof(Pawn), "playerSettings", "AreaRestriction");
        public static SyncField SyncMedCare = Sync.Field(typeof(Pawn), "playerSettings", "medCare");
        public static SyncField SyncSelfTend = Sync.Field(typeof(Pawn), "playerSettings", "selfTend");
        public static SyncField SyncHostilityResponse = Sync.Field(typeof(Pawn), "playerSettings", "hostilityResponse");
        public static SyncField SyncFollowFieldwork = Sync.Field(typeof(Pawn), "playerSettings", "followFieldwork");
        public static SyncField SyncFollowDrafted = Sync.Field(typeof(Pawn), "playerSettings", "followDrafted");
        public static SyncField SyncMaster = Sync.Field(typeof(Pawn), "playerSettings", "master");
        public static SyncField SyncGetsFood = Sync.Field(typeof(Pawn), "guest", "GetsFood");
        public static SyncField SyncInteractionMode = Sync.Field(typeof(Pawn), "guest", "interactionMode");

        public static SyncField SyncUseWorkPriorities = Sync.Field(null, "Verse.Current/Game/playSettings", "useWorkPriorities");
        public static SyncField SyncAutoHomeArea = Sync.Field(null, "Verse.Current/Game/playSettings", "autoHomeArea");
        public static SyncField[] SyncDefaultCare = Sync.Fields(
            null,
            "Verse.Find/World/settings",
            "defaultCareForColonyHumanlike",
            "defaultCareForColonyPrisoner",
            "defaultCareForColonyAnimal",
            "defaultCareForNeutralAnimal",
            "defaultCareForNeutralFaction",
            "defaultCareForHostileFaction"
        );

        public static SyncField[] SyncThingFilterHitPoints = Sync.FieldMultiTarget(Sync.thingFilterTarget, "AllowedHitPointsPercents");
        public static SyncField[] SyncThingFilterQuality = Sync.FieldMultiTarget(Sync.thingFilterTarget, "AllowedQualityLevels");

        [MpPrefix(typeof(AreaAllowedGUI), "DoAreaSelector")]
        static void DoAreaSelector_Prefix(Pawn p)
        {
            SyncAreaRestriction.Watch(p);
        }

        [MpPrefix(typeof(PawnColumnWorker_AllowedArea), "HeaderClicked")]
        static void AllowedArea_HeaderClicked_Prefix(PawnTable table)
        {
            foreach (Pawn pawn in table.PawnsListForReading)
                SyncAreaRestriction.Watch(pawn);
        }

        [MpPrefix("RimWorld.InspectPaneFiller+<DrawAreaAllowed>c__AnonStorey0", "<>m__0")]
        static void DrawAreaAllowed_Inner(object __instance)
        {
            SyncAreaRestriction.Watch(__instance.GetPropertyOrField("pawn"));
        }

        [MpPrefix(typeof(HealthCardUtility), "DrawOverviewTab")]
        static void HealthCardUtility1(Pawn pawn)
        {
            SyncMedCare.Watch(pawn);
            SyncSelfTend.Watch(pawn);
        }

        [MpPrefix(typeof(ITab_Pawn_Visitor), "FillTab")]
        static void ITab_Pawn_Visitor(ITab __instance)
        {
            Pawn pawn = __instance.GetPropertyOrField("SelPawn") as Pawn;
            SyncMedCare.Watch(pawn);
            SyncGetsFood.Watch(pawn);
            SyncInteractionMode.Watch(pawn);
        }

        [MpPrefix(typeof(HostilityResponseModeUtility), "DrawResponseButton")]
        static void DrawResponseButton(Pawn pawn)
        {
            SyncHostilityResponse.Watch(pawn);
        }

        [MpPrefix(typeof(PawnColumnWorker_FollowFieldwork), "SetValue")]
        static void FollowFieldwork(Pawn pawn)
        {
            SyncFollowFieldwork.Watch(pawn);
        }

        [MpPrefix(typeof(PawnColumnWorker_FollowDrafted), "SetValue")]
        static void FollowDrafted(Pawn pawn)
        {
            SyncFollowDrafted.Watch(pawn);
        }

        [MpPrefix(typeof(TrainableUtility), "<OpenMasterSelectMenu>c__AnonStorey0", "<>m__0")]
        static void OpenMasterSelectMenu_Inner1(object __instance)
        {
            SyncMaster.Watch(__instance.GetPropertyOrField("p"));
        }

        [MpPrefix(typeof(TrainableUtility), "<OpenMasterSelectMenu>c__AnonStorey1", "<>m__0")]
        static void OpenMasterSelectMenu_Inner2(object __instance)
        {
            SyncMaster.Watch(__instance.GetPropertyOrField("<>f__ref$0/p"));
        }

        [MpPrefix(typeof(Dialog_MedicalDefaults), "DoWindowContents")]
        static void MedicalDefaults()
        {
            SyncDefaultCare.Watch();
        }

        [MpPrefix(typeof(Widgets), "CheckboxLabeled")]
        static void CheckboxLabeled()
        {
            if (MethodMarkers.manualPriorities)
                SyncUseWorkPriorities.Watch();
        }

        [MpPrefix(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
        static void PlaySettingsControls()
        {
            SyncAutoHomeArea.Watch();
        }

        [MpPrefix(typeof(ThingFilterUI), "DrawHitPointsFilterConfig")]
        static void ThingFilterHitPoints()
        {
            if (MethodMarkers.tabStorage != null)
                SyncThingFilterHitPoints.Watch(MethodMarkers.tabStorage);
            else if (MethodMarkers.billConfig != null)
                SyncThingFilterHitPoints.Watch(MethodMarkers.billConfig);
        }

        [MpPrefix(typeof(ThingFilterUI), "DrawQualityFilterConfig")]
        static void ThingFilterQuality()
        {
            if (MethodMarkers.tabStorage != null)
                SyncThingFilterQuality.Watch(MethodMarkers.tabStorage);
            else if (MethodMarkers.billConfig != null)
                SyncThingFilterQuality.Watch(MethodMarkers.billConfig);
        }
    }

    public static class SyncPatches
    {
        public static SyncMethod SyncSetAssignment = Sync.Method(typeof(Pawn), "timetable", "SetAssignment");
        public static SyncMethod SyncSetWorkPriority = Sync.Method(typeof(Pawn), "workSettings", "SetPriority");
        public static SyncMethod SyncSetDrafted = Sync.Method(typeof(Pawn), "drafter", "set_Drafted");
        public static SyncMethod SyncSetFireAtWill = Sync.Method(typeof(Pawn), "drafter", "set_FireAtWill");
        public static SyncMethod[] SyncSetStoragePriority = Sync.MethodMultiTarget(Sync.storageTarget, "set_Priority");

        public static SyncMethod SyncStartJob = Sync.Method(typeof(Pawn), "jobs", "StartJob", typeof(Expose<Job>), typeof(JobCondition), typeof(ThinkNode), typeof(bool), typeof(bool), typeof(ThinkTreeDef), typeof(JobTag?), typeof(bool));
        public static SyncMethod SyncTryTakeOrderedJob = Sync.Method(typeof(Pawn), "jobs", "TryTakeOrderedJob", typeof(Expose<Job>), typeof(JobTag));
        public static SyncMethod SyncTryTakeOrderedJobPrioritizedWork = Sync.Method(typeof(Pawn), "jobs", "TryTakeOrderedJobPrioritizedWork", typeof(Expose<Job>), typeof(WorkGiver), typeof(IntVec3));

        public static SyncMethod SyncAddBill = Sync.Method(typeof(BillStack), "AddBill", typeof(Expose<Bill>));
        public static SyncMethod SyncDeleteBill = Sync.Method(typeof(BillStack), "Delete");
        public static SyncMethod SyncReorderBill = Sync.Method(typeof(BillStack), "Reorder");

        public static SyncField SyncTimetable = Sync.Field(typeof(Pawn), "timetable", "times");

        [MpPrefix(typeof(Pawn_TimetableTracker), "SetAssignment")]
        static bool SetTimetableAssignment(Pawn_TimetableTracker __instance, int hour, TimeAssignmentDef ta)
        {
            return !SyncSetAssignment.DoSync(__instance.GetPropertyOrField("pawn"), hour, ta);
        }

        [MpPrefix(typeof(PawnColumnWorker_CopyPasteTimetable), "PasteTo")]
        static bool CopyPasteTimetable(Pawn p)
        {
            return !SyncTimetable.DoSync(p, MpReflection.GetPropertyOrField("PawnColumnWorker_CopyPasteTimetable.clipboard"));
        }

        [MpPrefix(typeof(Pawn_WorkSettings), "SetPriority")]
        static bool SetWorkPriority(Pawn_WorkSettings __instance, WorkTypeDef w, int priority)
        {
            return !SyncSetWorkPriority.DoSync(__instance.GetPropertyOrField("pawn"), w, priority);
        }

        [MpPrefix(typeof(Pawn_DraftController), "set_Drafted")]
        static bool SetDrafted(Pawn_DraftController __instance, bool value)
        {
            return !SyncSetDrafted.DoSync(__instance.pawn, value);
        }

        [MpPrefix(typeof(Pawn_DraftController), "set_FireAtWill")]
        static bool SetFireAtWill(Pawn_DraftController __instance, bool value)
        {
            return !SyncSetFireAtWill.DoSync(__instance.pawn, value);
        }

        [MpPrefix(typeof(Pawn_JobTracker), "StartJob")]
        static bool StartJob(Pawn_JobTracker __instance, Job newJob, JobCondition lastJobEndCondition, ThinkNode jobGiver, bool resumeCurJobAfterwards, bool cancelBusyStances, ThinkTreeDef thinkTree, JobTag? tag, bool fromQueue)
        {
            return !SyncSetFireAtWill.DoSync(__instance.GetPropertyOrField("pawn"), newJob, lastJobEndCondition, jobGiver, resumeCurJobAfterwards, cancelBusyStances, thinkTree, tag, fromQueue);
        }

        [MpPrefix(typeof(StorageSettings), "set_Priority")]
        static bool StorageSetPriority(StorageSettings __instance, StoragePriority value)
        {
            return !SyncSetStoragePriority.DoSync(__instance.owner, value);
        }

        [MpPrefix(typeof(Pawn_JobTracker), "TryTakeOrderedJob")]
        static bool TryTakeOrderedJob(Pawn_JobTracker __instance, Job job, JobTag tag)
        {
            return !SyncTryTakeOrderedJob.DoSync(__instance.GetPropertyOrField("pawn"), job, tag);
        }

        [MpPrefix(typeof(Pawn_JobTracker), "TryTakeOrderedJobPrioritizedWork")]
        static bool TryTakeOrderedJobPrioritizedWork(Pawn_JobTracker __instance, Job job, WorkGiver giver, IntVec3 cell)
        {
            return !SyncTryTakeOrderedJobPrioritizedWork.DoSync(__instance.GetPropertyOrField("pawn"), job, giver, cell);
        }

        [MpPrefix(typeof(BillStack), "AddBill")]
        static bool AddBill(BillStack __instance, Bill bill)
        {
            return !SyncAddBill.DoSync(__instance, bill);
        }

        [MpPrefix(typeof(BillStack), "Delete")]
        static bool DeleteBill(BillStack __instance, Bill bill)
        {
            return !SyncDeleteBill.DoSync(__instance, bill);
        }

        [MpPrefix(typeof(BillStack), "Reorder")]
        static bool ReorderBill(BillStack __instance, Bill bill, int offset)
        {
            return !SyncReorderBill.DoSync(__instance, bill, offset);
        }
    }

    public static class SyncDelegates
    {
        [SyncDelegate]
        [MpPrefix(typeof(FloatMenuMakerMap), "<GotoLocationOption>c__AnonStorey18", "<>m__0")] // Goto
        [MpPrefix(typeof(FloatMenuMakerMap), "<AddDraftedOrders>c__AnonStorey3", "<>m__0")] // Arrest
        [MpPrefix(typeof(FloatMenuMakerMap), "<AddHumanlikeOrders>c__AnonStorey7", "<>m__0")] // Rescue
        [MpPrefix(typeof(FloatMenuMakerMap), "<AddHumanlikeOrders>c__AnonStorey7", "<>m__1")] // Capture
        [MpPrefix(typeof(FloatMenuMakerMap), "<AddHumanlikeOrders>c__AnonStorey9", "<>m__0")] // Carry to cryptosleep casket
        static bool FloatMenuGeneral(object __instance)
        {
            return !Sync.Delegate(__instance, MapProviderMode.ANY_FIELD);
        }

        [SyncDelegate("$this")]
        [MpPrefix(typeof(Pawn_PlayerSettings), "<GetGizmos>c__Iterator0", "<>m__2")]
        static bool GizmoReleaseAnimals(object __instance)
        {
            return !Sync.Delegate(__instance, __instance.GetPropertyOrField("$this/pawn"));
        }

        [SyncDelegate("$this")]
        [MpPrefix(typeof(PriorityWork), "<GetGizmos>c__Iterator0", "<>m__0")]
        static bool GizmoClearPrioritizedWork(object __instance)
        {
            return !Sync.Delegate(__instance, __instance.GetPropertyOrField("$this/pawn"));
        }

        /*[SyncDelegate("lord")]
        [MpPatch(typeof(Pawn_MindState), "<GetGizmos>c__Iterator0+<GetGizmos>c__AnonStorey2", "<>m__0")]
        static bool GizmoCancelFormingCaravan(object __instance)
        {
            if (!Multiplayer.ShouldSync) return true;
            Sync.Delegate(__instance, MpReflection.GetPropertyOrField(__instance, ""));
            return false;
        }*/
    }

    public static class MapProviderMode
    {
        public static readonly object ANY_FIELD = new object();
    }

    public class SyncDelegateAttribute : Attribute
    {
        public readonly string[] fields;

        public SyncDelegateAttribute()
        {
        }

        public SyncDelegateAttribute(params string[] fields)
        {
            this.fields = fields;
        }
    }

    public static class MethodMarkers
    {
        public static bool manualPriorities;
        public static IStoreSettingsParent tabStorage;
        public static Bill billConfig;
        public static Outfit dialogOutfit;

        [MpPrefix(typeof(MainTabWindow_Work), "DoManualPrioritiesCheckbox")]
        static void ManualPriorities_Prefix() => manualPriorities = true;

        [MpPostfix(typeof(MainTabWindow_Work), "DoManualPrioritiesCheckbox")]
        static void ManualPriorities_Postfix() => manualPriorities = false;

        [MpPrefix(typeof(ITab_Storage), "FillTab")]
        static void TabStorageFillTab_Prefix(ITab_Storage __instance) => tabStorage = (IStoreSettingsParent)__instance.GetPropertyOrField("SelStoreSettingsParent");

        [MpPostfix(typeof(ITab_Storage), "FillTab")]
        static void TabStorageFillTab_Postfix() => tabStorage = null;

        [MpPrefix(typeof(Dialog_BillConfig), "DoWindowContents")]
        static void BillConfig_Prefix(Dialog_BillConfig __instance) => billConfig = (Bill)__instance.GetPropertyOrField("bill");

        [MpPostfix(typeof(Dialog_BillConfig), "DoWindowContents")]
        static void BillConfig_Postfix() => billConfig = null;

        [MpPrefix(typeof(Dialog_ManageOutfits), "DoWindowContents")]
        static void ManageOutfit_Prefix(Dialog_ManageOutfits __instance) => dialogOutfit = (Outfit)__instance.GetPropertyOrField("SelectedOutfit");

        [MpPostfix(typeof(Dialog_ManageOutfits), "DoWindowContents")]
        static void ManageOutfit_Postfix() => billConfig = null;
    }

    public abstract class SyncHandler
    {
        public readonly int syncId;

        public SyncHandler(int syncId)
        {
            this.syncId = syncId;
        }

        public abstract void Read(ByteReader data);
    }

    public class SyncField : SyncHandler
    {
        public readonly Type targetType;
        public readonly string memberPath;
        public readonly Type fieldType;
        public readonly Func<object, object> instanceFunc;

        public SyncField(int syncId, Type targetType, string memberPath) : base(syncId)
        {
            this.targetType = targetType;
            this.memberPath = targetType + "/" + memberPath;

            fieldType = MpReflection.PropertyOrFieldType(this.memberPath);
        }

        /// <summary>
        /// Returns whether the sync has been done
        /// </summary>
        public bool DoSync(object target, object value)
        {
            if (!Multiplayer.ShouldSync)
                return false;

            int mapId = Sync.GetMap(target)?.uniqueID ?? ScheduledCommand.GLOBAL;
            ByteWriter writer = new ByteWriter();

            writer.WriteInt32(syncId);

            Sync.WriteContext(writer, mapId);
            Sync.WriteSyncObject(writer, target, targetType);
            Sync.WriteSyncObject(writer, value, fieldType);

            Multiplayer.client.SendCommand(CommandType.SYNC, mapId, writer.GetArray());

            return true;
        }

        public override void Read(ByteReader data)
        {
            object target = Sync.ReadSyncObject(data, targetType);
            object value = Sync.ReadSyncObject(data, fieldType);

            MpLog.Log("Set " + memberPath + " in " + target + " to " + value + " map " + data.context);
            MpReflection.SetPropertyOrField(target, memberPath, value);
        }
    }

    public class SyncMethod : SyncHandler
    {
        public readonly Type targetType;
        public readonly string instancePath;

        public readonly MethodInfo method;
        public readonly Type[] argTypes;

        public SyncMethod(int syncId, Type targetType, string instancePath, string methodName, Type[] argTypes) : base(syncId)
        {
            this.targetType = targetType;

            Type instanceType = targetType;
            if (!instancePath.NullOrEmpty())
            {
                this.instancePath = instanceType + "/" + instancePath;
                instanceType = MpReflection.PropertyOrFieldType(this.instancePath);
            }

            method = AccessTools.Method(instanceType, methodName) ?? throw new Exception($"Couldn't find method {instanceType}::{methodName}");

            if (argTypes.Length == 0)
                this.argTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
            else if (argTypes.Length != method.GetParameters().Length)
                throw new Exception("Wrong parameter count for method " + method);
            else
                this.argTypes = argTypes;
        }

        /// <summary>
        /// Returns whether the sync has been done
        /// </summary>
        public bool DoSync(object target, params object[] args)
        {
            if (!Multiplayer.ShouldSync)
                return false;

            int mapId = Sync.GetMap(target)?.uniqueID ?? ScheduledCommand.GLOBAL;
            ByteWriter writer = new ByteWriter();

            writer.WriteInt32(syncId);

            Sync.WriteContext(writer, mapId);
            Sync.WriteSyncObject(writer, target, targetType);
            Sync.WriteSyncObjects(writer, args, argTypes);

            Multiplayer.client.SendCommand(CommandType.SYNC, mapId, writer.GetArray());

            return true;
        }

        public override void Read(ByteReader data)
        {
            object target = Sync.ReadSyncObject(data, targetType);
            if (!instancePath.NullOrEmpty())
                target = target.GetPropertyOrField(instancePath);

            object[] parameters = Sync.ReadSyncObjects(data, argTypes);

            MpLog.Log("Invoked " + method + " on " + target + " with " + parameters.Length + " params");
            method.Invoke(target, parameters);
        }
    }

    public class SyncDelegate : SyncHandler
    {
        public readonly Type delegateType;
        public readonly MethodInfo method;

        private Type[] argTypes;
        public string[] fieldPaths;
        private Type[] fieldTypes;

        public SyncDelegate(int syncId, Type delegateType, string delegateMethod, string[] fieldPaths) : base(syncId)
        {
            this.delegateType = delegateType;
            method = AccessTools.Method(delegateType, delegateMethod);

            argTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

            this.fieldPaths = fieldPaths;
            if (fieldPaths == null)
            {
                List<string> fieldList = new List<string>();
                Sync.AllDelegateFieldsRecursive(delegateType, path => { fieldList.Add(path); return false; });
                this.fieldPaths = fieldList.ToArray();
            }
            else
            {
                for (int i = 0; i < this.fieldPaths.Length; i++)
                {
                    this.fieldPaths[i] = MpReflection.AppendType(this.fieldPaths[i], delegateType);
                }
            }

            fieldTypes = this.fieldPaths.Select(path => MpReflection.PropertyOrFieldType(path)).ToArray();
        }

        public bool DoSync(object delegateInstance, int mapId, params object[] args)
        {
            if (!Multiplayer.ShouldSync)
                return false;

            ByteWriter writer = new ByteWriter();
            writer.WriteInt32(syncId);

            Sync.WriteContext(writer, mapId);

            foreach (string field in fieldPaths)
                Sync.WriteSyncObject(writer, delegateInstance.GetPropertyOrField(field));

            Sync.WriteSyncObjects(writer, args, argTypes);

            Multiplayer.client.SendCommand(CommandType.SYNC, mapId, writer.GetArray());

            return true;
        }

        public override void Read(ByteReader data)
        {
            object target = Activator.CreateInstance(delegateType);
            for (int i = 0; i < fieldPaths.Length; i++)
                MpReflection.SetPropertyOrField(target, fieldPaths[i], Sync.ReadSyncObject(data, fieldTypes[i]));

            object[] parameters = Sync.ReadSyncObjects(data, argTypes);

            MpLog.Log("Invoked delegate method " + method + " " + delegateType);
            method.Invoke(target, parameters);
        }
    }

    public struct SyncData
    {
        public readonly object target;
        public readonly SyncField handler;
        public readonly object value;

        public SyncData(object target, SyncField handler, object value)
        {
            this.target = target;
            this.handler = handler;
            this.value = value;
        }
    }

    public class Expose<T> { }

    public static class Sync
    {
        private static List<SyncHandler> handlers = new List<SyncHandler>();
        private static Dictionary<MethodBase, SyncDelegate> delegates = new Dictionary<MethodBase, SyncDelegate>();

        private static List<SyncData> watchedData = new List<SyncData>();
        private static bool syncing;

        public static MultiTarget storageTarget = new MultiTarget()
        {
            { typeof(Building_Grave), "GetStoreSettings" },
            { typeof(Building_Storage), "GetStoreSettings" },
            { typeof(CompChangeableProjectile), "GetStoreSettings" },
            { typeof(Zone_Stockpile), "GetStoreSettings" }
        };

        public static MultiTarget thingFilterTarget = new MultiTarget()
        {
            { storageTarget, "filter" },
            { typeof(Bill), "ingredientFilter" },
            { typeof(Outfit), "filter" }
        };

        private static void Prefix(ref bool __state)
        {
            if (!syncing && Multiplayer.ShouldSync)
            {
                syncing = __state = true;
            }
        }

        private static void Postfix(ref bool __state)
        {
            if (!__state)
                return;

            foreach (SyncData data in watchedData)
            {
                object newValue = data.target.GetPropertyOrField(data.handler.memberPath);

                if (!Equals(newValue, data.value))
                {
                    MpReflection.SetPropertyOrField(data.target, data.handler.memberPath, data.value);
                    data.handler.DoSync(data.target, newValue);
                }
            }

            watchedData.Clear();
            syncing = __state = false;
        }

        public static SyncMethod Method(Type targetType, string methodName)
        {
            return Method(targetType, null, methodName);
        }

        public static SyncMethod Method(Type targetType, string methodName, params Type[] argTypes)
        {
            return Method(targetType, null, methodName, argTypes);
        }

        public static SyncMethod Method(Type targetType, string instancePath, string methodName, params Type[] argTypes)
        {
            SyncMethod handler = new SyncMethod(handlers.Count, targetType, instancePath, methodName, argTypes);
            handlers.Add(handler);
            return handler;
        }

        public static SyncMethod[] MethodMultiTarget(MultiTarget targetType, string methodName)
        {
            return targetType.Select(type => Method(type.First, type.Second, methodName)).ToArray();
        }

        public static SyncField Field(Type targetType, string instancePath, string fieldName)
        {
            SyncField handler = new SyncField(handlers.Count, targetType, instancePath + "/" + fieldName);
            handlers.Add(handler);
            return handler;
        }

        public static SyncField[] FieldMultiTarget(MultiTarget targetType, string fieldName)
        {
            return targetType.Select(type => Field(type.First, type.Second, fieldName)).ToArray();
        }

        public static SyncField[] Fields(Type targetType, string instancePath, params string[] memberPaths)
        {
            return memberPaths.Select(path => Field(targetType, instancePath, path)).ToArray();
        }

        /// <summary>
        /// Returns whether the sync has been done
        /// </summary>
        public static bool Delegate(object instance, object mapProvider, params object[] args)
        {
            MethodBase caller = new StackTrace().GetFrame(1).GetMethod();
            SyncDelegate handler = delegates[caller];
            int mapId = GetMap(mapProvider)?.uniqueID ?? ScheduledCommand.GLOBAL;

            if (mapProvider == MapProviderMode.ANY_FIELD)
            {
                foreach (string path in handler.fieldPaths)
                {
                    object obj = instance.GetPropertyOrField(path);
                    Map map = GetMap(obj);
                    if (map != null)
                    {
                        mapId = map.uniqueID;
                        break;
                    }
                }
            }

            args = args ?? new object[0];
            return handler.DoSync(instance, mapId, args);
        }

        public static bool AllDelegateFieldsRecursive(Type type, Func<string, bool> getter, string path = "")
        {
            if (path.NullOrEmpty())
                path = type.ToString();

            foreach (FieldInfo field in AccessTools.GetDeclaredFields(type))
            {
                string curPath = path + "/" + field.Name;

                if (getter(curPath))
                    return true;

                if (Attribute.GetCustomAttribute(field.FieldType, typeof(CompilerGeneratedAttribute)) == null)
                    continue;

                if (AllDelegateFieldsRecursive(field.FieldType, getter, curPath))
                    return true;
            }

            return false;
        }

        public static void RegisterSyncDelegates(Type inType)
        {
            foreach (MethodInfo method in AccessTools.GetDeclaredMethods(inType))
            {
                if (!method.TryGetAttribute(out SyncDelegateAttribute syncAttr))
                    continue;

                foreach (MpPrefix patchAttr in method.AllAttributes<MpPrefix>())
                {
                    Type type = patchAttr.type ?? MpReflection.GetTypeByName(patchAttr.typeName);
                    SyncDelegate handler = new SyncDelegate(handlers.Count, type, patchAttr.method, syncAttr.fields);
                    delegates[method] = handler;
                    handlers.Add(handler);
                }
            }
        }

        public static bool CanSerialize(Type type)
        {
            return type.GetConstructors(AccessTools.all).Any(c => c.GetParameters().Length == 0);
        }

        public static void RegisterFieldPatches(Type type)
        {
            HarmonyMethod prefix = new HarmonyMethod(AccessTools.Method(typeof(Sync), "Prefix"));
            HarmonyMethod postfix = new HarmonyMethod(AccessTools.Method(typeof(Sync), "Postfix"));

            List<MethodBase> patched = MpPatch.DoPatches(type);
            new PatchProcessor(Multiplayer.harmony, patched, prefix, postfix).Patch();
        }

        public static void Watch(this SyncField field, object target = null)
        {
            if (!Multiplayer.ShouldSync) return;

            object value = target.GetPropertyOrField(field.memberPath);
            watchedData.Add(new SyncData(target, field, value));
        }

        public static void Watch(this SyncField[] group, object target = null)
        {
            foreach (SyncField sync in group)
                if (target == null || sync.targetType.IsAssignableFrom(target.GetType()))
                    sync.Watch(target);
        }

        public static bool DoSync(this SyncMethod[] group, object target, params object[] args)
        {
            foreach (SyncMethod sync in group)
                if (target == null || sync.targetType.IsAssignableFrom(target.GetType()))
                    return sync.DoSync(target);

            return false;
        }

        public static void HandleCmd(ByteReader data)
        {
            int syncId = data.ReadInt32();
            SyncHandler handler = handlers[syncId];

            if (data.context is Map)
            {
                IntVec3 mouseCell = ReadSync<IntVec3>(data);
                MouseCellPatch.result = mouseCell;
            }

            bool shouldQueue = data.ReadBool();
            KeyIsDownPatch.result = shouldQueue;
            KeyIsDownPatch.forKey = KeyBindingDefOf.QueueOrder;

            handler.Read(data);

            MouseCellPatch.result = null;
            KeyIsDownPatch.result = null;
            KeyIsDownPatch.forKey = null;
        }

        public static void WriteContext(ByteWriter data, int mapId)
        {
            if (mapId >= 0)
                WriteSync(data, UI.MouseCell());

            data.WriteBool(KeyBindingDefOf.QueueOrder.IsDownEvent);
        }

        public static Map GetMap(object obj)
        {
            if (obj is Thing thing)
                return thing.Map;
            else if (obj is ThingComp comp)
                return comp.parent.Map;
            else if (obj is Zone zone)
                return zone.Map;
            else if (obj is Bill bill)
                return bill.Map;
            else if (obj is BillStack bills)
                return bills.billGiver.Map;

            return null;
        }

        static ReaderDictionary readers = new ReaderDictionary
        {
            { data => data.ReadInt32() },
            { data => data.ReadBool() },
            { data => data.ReadString() },
            { data => data.ReadLong() },
            { data => new IntVec3(data.ReadInt32(), data.ReadInt32(), data.ReadInt32()) },
            { data => ReadSync<Pawn>(data).mindState.priorityWork },
            { data => ReadSync<Pawn>(data).playerSettings },
            { data => new FloatRange(data.ReadFloat(), data.ReadFloat()) },
            { data => new IntRange(data.ReadInt32(), data.ReadInt32()) },
            { data => new QualityRange(ReadSync<QualityCategory>(data), ReadSync<QualityCategory>(data)) },
            { data =>
            {
                Thing thing = ReadSync<Thing>(data);
                if (thing != null)
                    return new LocalTargetInfo(thing);
                else
                    return new LocalTargetInfo(ReadSync<IntVec3>(data));
            }
            }
        };

        static WriterDictionary writers = new WriterDictionary
        {
            { (ByteWriter data, int i) => data.WriteInt32(i) },
            { (ByteWriter data, bool b) => data.WriteBool(b) },
            { (ByteWriter data, string s) => data.WriteString(s) },
            { (ByteWriter data, long l) => data.WriteLong(l) },
            { (ByteWriter data, PriorityWork work) => WriteSyncObject(data, work.GetPropertyOrField("pawn")) },
            { (ByteWriter data, Pawn_PlayerSettings settings) => WriteSyncObject(data, settings.GetPropertyOrField("pawn")) },
            { (ByteWriter data, FloatRange range) => { data.WriteFloat(range.min); data.WriteFloat(range.max); }},
            { (ByteWriter data, IntRange range) => { data.WriteInt32(range.min); data.WriteInt32(range.max); }},
            { (ByteWriter data, QualityRange range) => { WriteSync(data, range.min); WriteSync(data, range.max); }},
            { (ByteWriter data, IntVec3 vec) => { data.WriteInt32(vec.x); data.WriteInt32(vec.y); data.WriteInt32(vec.z); }},
            {
                (ByteWriter data, LocalTargetInfo info) =>
                {
                    WriteSync(data, info.Thing);
                    if (!info.HasThing)
                        WriteSync(data, info.Cell);
                }
            }
        };

        public static T ReadSync<T>(ByteReader data)
        {
            return (T)ReadSyncObject(data, typeof(T));
        }

        private static MethodInfo ReadExposable = AccessTools.Method(typeof(ScribeUtil), "ReadExposable");

        public static object ReadSyncObject(ByteReader data, Type type)
        {
            Map map = data.context as Map;

            if (type.IsEnum)
            {
                return Enum.ToObject(type, data.ReadInt32());
            }
            else if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type listType = type.GetGenericArguments()[0];
                    int size = data.ReadInt32();
                    IList list = Activator.CreateInstance(type, size) as IList;
                    for (int j = 0; j < size; j++)
                        list.Add(ReadSyncObject(data, listType));
                    return list;
                }
                else if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    bool hasValue = data.ReadBool();
                    if (!hasValue)
                        return Activator.CreateInstance(type);

                    Type nullableType = type.GetGenericArguments()[0];
                    return Activator.CreateInstance(type, ReadSyncObject(data, nullableType));
                }
                else if (type.GetGenericTypeDefinition() == typeof(Expose<>))
                {
                    Type exposableType = type.GetGenericArguments()[0];
                    byte[] exposableData = data.ReadPrefixedBytes();
                    return ReadExposable.MakeGenericMethod(exposableType).Invoke(null, new[] { exposableData, null });
                }
            }
            else if (typeof(ThinkNode).IsAssignableFrom(type))
            {
                // todo temporary
                return null;
            }
            else if (typeof(Area).IsAssignableFrom(type))
            {
                int areaId = data.ReadInt32();
                if (areaId == -1)
                    return null;

                return map.areaManager.AllAreas.FirstOrDefault(a => a.ID == areaId);
            }
            else if (typeof(Zone).IsAssignableFrom(type))
            {
                string name = data.ReadString();
                Log.Message("Reading zone " + name + " " + map);
                if (name.NullOrEmpty())
                    return null;

                return map.zoneManager.AllZones.FirstOrDefault(zone => zone.label == name);
            }
            else if (typeof(Def).IsAssignableFrom(type))
            {
                ushort shortHash = data.ReadUInt16();
                if (shortHash == 0)
                    return null;

                Type dbType = typeof(DefDatabase<>).MakeGenericType(type);
                return AccessTools.Method(dbType, "GetByShortHash").Invoke(null, new object[] { shortHash });
            }
            else if (typeof(Thing).IsAssignableFrom(type))
            {
                int thingId = data.ReadInt32();
                if (thingId == -1)
                    return null;

                ThingDef def = ReadSync<ThingDef>(data);
                return map.listerThings.ThingsOfDef(def).FirstOrDefault(t => t.thingIDNumber == thingId);
            }
            else if (typeof(ThingComp).IsAssignableFrom(type))
            {
                string compType = data.ReadString();
                if (compType.NullOrEmpty())
                    return null;

                ThingWithComps parent = ReadSync<Thing>(data) as ThingWithComps;
                if (parent == null)
                    return null;

                return parent.AllComps.FirstOrDefault(comp => comp.props.compClass.FullName == compType);
            }
            else if (typeof(WorkGiver).IsAssignableFrom(type))
            {
                WorkGiverDef def = ReadSync<WorkGiverDef>(data);
                return def?.Worker;
            }
            else if (typeof(BillStack) == type)
            {
                Thing thing = ReadSync<Thing>(data);
                if (thing is IBillGiver billGiver)
                    return billGiver.BillStack;
                return null;
            }
            else if (typeof(Bill).IsAssignableFrom(type))
            {
                BillStack billStack = ReadSync<BillStack>(data);
                if (billStack == null)
                    return null;

                int id = data.ReadInt32();
                return billStack.Bills.FirstOrDefault(bill => (int)bill.GetPropertyOrField("loadID") == id);
            }
            else if (readers.TryGetValue(type, out Func<ByteReader, object> reader))
            {
                return reader(data);
            }

            throw new SerializationException("No reader for type " + type);
        }

        public static object[] ReadSyncObjects(ByteReader data, Type[] spec)
        {
            return spec.Select(type => ReadSyncObject(data, type)).ToArray();
        }

        public static void WriteSync<T>(ByteWriter data, T obj)
        {
            WriteSyncObject(data, obj, typeof(T));
        }

        public static void WriteSyncObject(ByteWriter data, object obj)
        {
            WriteSyncObject(data, obj, obj.GetType());
        }

        public static void WriteSyncObject(ByteWriter data, object obj, Type type)
        {
            if (type.IsEnum)
            {
                data.WriteInt32(Convert.ToInt32(obj));
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type listType = type.GetGenericArguments()[0];
                IList list = obj as IList;
                data.WriteInt32(list.Count);
                foreach (object e in list)
                    WriteSyncObject(data, e, listType);
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type nullableType = type.GetGenericArguments()[0];
                bool hasValue = (bool)obj.GetPropertyOrField("HasValue");

                data.WriteBool(hasValue);
                if (hasValue)
                    WriteSyncObject(data, obj.GetPropertyOrField("Value"));
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expose<>))
            {
                Type exposableType = type.GetGenericArguments()[0];
                if (!exposableType.IsAssignableFrom(obj.GetType()))
                    throw new SerializationException($"Expose<> types {obj.GetType()} and {exposableType} don't match");

                IExposable exposable = obj as IExposable;
                data.WritePrefixedBytes(ScribeUtil.WriteExposable(exposable));
            }
            else if (typeof(ThinkNode).IsAssignableFrom(type))
            {
                // todo temporary
            }
            else if (typeof(Area).IsAssignableFrom(type))
            {
                data.WriteInt32(obj is Area area ? area.ID : -1);
            }
            else if (typeof(Zone).IsAssignableFrom(type))
            {
                data.WriteString(obj is Zone zone ? zone.label : "");
            }
            else if (typeof(Def).IsAssignableFrom(type))
            {
                data.WriteUInt16(obj is Def def ? def.shortHash : (ushort)0);
            }
            else if (typeof(ThingComp).IsAssignableFrom(type))
            {
                if (obj is ThingComp comp)
                {
                    data.WriteString(comp.props.compClass.FullName);
                    WriteSync<Thing>(data, comp.parent);
                }
                else
                {
                    data.WriteString("");
                }
            }
            else if (typeof(WorkGiver).IsAssignableFrom(type))
            {
                WorkGiver workGiver = obj as WorkGiver;
                if (workGiver == null)
                    return;

                WriteSync(data, workGiver);
            }
            else if (typeof(Thing).IsAssignableFrom(type))
            {
                Thing thing = (Thing)obj;
                if (thing == null)
                {
                    data.WriteInt32(-1);
                    return;
                }

                data.WriteInt32(thing.thingIDNumber);
                WriteSync(data, thing.def);
            }
            else if (typeof(BillStack) == type)
            {
                Thing billGiver = (obj as BillStack)?.billGiver as Thing;
                WriteSync(data, billGiver);
            }
            else if (typeof(Bill) == type)
            {
                Bill bill = obj as Bill;
                WriteSync(data, bill.billStack);
                data.WriteInt32((int)bill.GetPropertyOrField("loadID"));
            }
            else if (writers.TryGetValue(type, out Action<ByteWriter, object> writer))
            {
                writer(data, obj);
            }
            else
            {
                throw new SerializationException("No writer for type " + type);
            }
        }

        public static void WriteSyncObjects(ByteWriter data, object[] objs, Type[] spec)
        {
            for (int i = 0; i < spec.Length; i++)
                WriteSyncObject(data, objs[i], spec[i]);
        }
    }

    public class MultiTarget : IEnumerable<MultiType>
    {
        private List<MultiType> types = new List<MultiType>();

        public void Add(Type type, string path)
        {
            types.Add(new MultiType(type, path));
        }

        public void Add(MultiTarget type, string path)
        {
            foreach (MultiType multiType in type)
                Add(multiType.First, multiType.Second + "/" + path);
        }

        public IEnumerator<MultiType> GetEnumerator()
        {
            return types.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return types.GetEnumerator();
        }
    }

    class ReaderDictionary : OrderedDictionary<Type, Func<ByteReader, object>>
    {
        public void Add<T>(Func<ByteReader, T> writer)
        {
            Add(typeof(T), data => writer(data));
        }
    }

    class WriterDictionary : OrderedDictionary<Type, Action<ByteWriter, object>>
    {
        public void Add<T>(Action<ByteWriter, T> writer)
        {
            Add(typeof(T), (data, o) => writer(data, (T)o));
        }
    }

    abstract class OrderedDictionary<K, V> : IEnumerable
    {
        private OrderedDictionary dict = new OrderedDictionary();

        protected void Add(K key, V value)
        {
            dict.Add(key, value);
        }

        public bool TryGetValue(K key, out V value)
        {
            value = (V)dict[key];
            return value != null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }
    }

    public class SerializationException : Exception
    {
        public SerializationException(string msg) : base(msg)
        {
        }
    }

}
