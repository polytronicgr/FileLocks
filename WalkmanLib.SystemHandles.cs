﻿// Get all system open handles method - uses NTQuerySystemInformation and NTQueryObject
//https://gist.github.com/i-e-b/2290426
//https://stackoverflow.com/a/13735033/2999220
//https://stackoverflow.com/a/6351168/2999220


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WalkmanLib
{
    class SystemHandles
    {
        #region Native Methods

        #region Enums

        //https://pinvoke.net/default.aspx/Enums.NtStatus
        //https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-erref/596a1078-e883-4972-9bbc-49e60bebca55
        protected enum NTSTATUS : uint
        {
            STATUS_SUCCESS =              0x00000000,
            STATUS_BUFFER_OVERFLOW =      0x80000005,
            STATUS_INFO_LENGTH_MISMATCH = 0xC0000004
        }

        //https://www.pinvoke.net/default.aspx/ntdll/SYSTEM_INFORMATION_CLASS.html
        protected enum SYSTEM_INFORMATION_CLASS
        {
            SystemBasicInformation =                                0x00,
            SystemProcessorInformation =                            0x01,
            SystemPerformanceInformation =                          0x02,
            SystemTimeOfDayInformation =                            0x03,
            SystemPathInformation =                                 0x04,
            SystemProcessInformation =                              0x05,
            SystemCallCountInformation =                            0x06,
            SystemDeviceInformation =                               0x07,
            SystemProcessorPerformanceInformation =                 0x08,
            SystemFlagsInformation =                                0x09,
            SystemCallTimeInformation =                             0x0A,
            SystemModuleInformation =                               0x0B,
            SystemLocksInformation =                                0x0C,
            SystemStackTraceInformation =                           0x0D,
            SystemPagedPoolInformation =                            0x0E,
            SystemNonPagedPoolInformation =                         0x0F,
            SystemHandleInformation =                               0x10,
            SystemObjectInformation =                               0x11,
            SystemPageFileInformation =                             0x12,
            SystemVdmInstemulInformation =                          0x13,
            SystemVdmBopInformation =                               0x14,
            SystemFileCacheInformation =                            0x15,
            SystemPoolTagInformation =                              0x16,
            SystemInterruptInformation =                            0x17,
            SystemDpcBehaviorInformation =                          0x18,
            SystemFullMemoryInformation =                           0x19,
            SystemLoadGdiDriverInformation =                        0x1A,
            SystemUnloadGdiDriverInformation =                      0x1B,
            SystemTimeAdjustmentInformation =                       0x1C,
            SystemSummaryMemoryInformation =                        0x1D,
            SystemMirrorMemoryInformation =                         0x1E,
            SystemPerformanceTraceInformation =                     0x1F,
            SystemObsolete0 =                                       0x20,
            SystemExceptionInformation =                            0x21,
            SystemCrashDumpStateInformation =                       0x22,
            SystemKernelDebuggerInformation =                       0x23,
            SystemContextSwitchInformation =                        0x24,
            SystemRegistryQuotaInformation =                        0x25,
            SystemExtendServiceTableInformation =                   0x26,
            SystemPrioritySeperation =                              0x27,
            SystemVerifierAddDriverInformation =                    0x28,
            SystemVerifierRemoveDriverInformation =                 0x29,
            SystemProcessorIdleInformation =                        0x2A,
            SystemLegacyDriverInformation =                         0x2B,
            SystemCurrentTimeZoneInformation =                      0x2C,
            SystemLookasideInformation =                            0x2D,
            SystemTimeSlipNotification =                            0x2E,
            SystemSessionCreate =                                   0x2F,
            SystemSessionDetach =                                   0x30,
            SystemSessionInformation =                              0x31,
            SystemRangeStartInformation =                           0x32,
            SystemVerifierInformation =                             0x33,
            SystemVerifierThunkExtend =                             0x34,
            SystemSessionProcessInformation =                       0x35,
            SystemLoadGdiDriverInSystemSpace =                      0x36,
            SystemNumaProcessorMap =                                0x37,
            SystemPrefetcherInformation =                           0x38,
            SystemExtendedProcessInformation =                      0x39,
            SystemRecommendedSharedDataAlignment =                  0x3A,
            SystemComPlusPackage =                                  0x3B,
            SystemNumaAvailableMemory =                             0x3C,
            SystemProcessorPowerInformation =                       0x3D,
            SystemEmulationBasicInformation =                       0x3E,
            SystemEmulationProcessorInformation =                   0x3F,
            SystemExtendedHandleInformation =                       0x40,
            SystemLostDelayedWriteInformation =                     0x41,
            SystemBigPoolInformation =                              0x42,
            SystemSessionPoolTagInformation =                       0x43,
            SystemSessionMappedViewInformation =                    0x44,
            SystemHotpatchInformation =                             0x45,
            SystemObjectSecurityMode =                              0x46,
            SystemWatchdogTimerHandler =                            0x47,
            SystemWatchdogTimerInformation =                        0x48,
            SystemLogicalProcessorInformation =                     0x49,
            SystemWow64SharedInformationObsolete =                  0x4A,
            SystemRegisterFirmwareTableInformationHandler =         0x4B,
            SystemFirmwareTableInformation =                        0x4C,
            SystemModuleInformationEx =                             0x4D,
            SystemVerifierTriageInformation =                       0x4E,
            SystemSuperfetchInformation =                           0x4F,
            SystemMemoryListInformation =                           0x50,
            SystemFileCacheInformationEx =                          0x51,
            SystemThreadPriorityClientIdInformation =               0x52,
            SystemProcessorIdleCycleTimeInformation =               0x53,
            SystemVerifierCancellationInformation =                 0x54,
            SystemProcessorPowerInformationEx =                     0x55,
            SystemRefTraceInformation =                             0x56,
            SystemSpecialPoolInformation =                          0x57,
            SystemProcessIdInformation =                            0x58,
            SystemErrorPortInformation =                            0x59,
            SystemBootEnvironmentInformation =                      0x5A,
            SystemHypervisorInformation =                           0x5B,
            SystemVerifierInformationEx =                           0x5C,
            SystemTimeZoneInformation =                             0x5D,
            SystemImageFileExecutionOptionsInformation =            0x5E,
            SystemCoverageInformation =                             0x5F,
            SystemPrefetchPatchInformation =                        0x60,
            SystemVerifierFaultsInformation =                       0x61,
            SystemSystemPartitionInformation =                      0x62,
            SystemSystemDiskInformation =                           0x63,
            SystemProcessorPerformanceDistribution =                0x64,
            SystemNumaProximityNodeInformation =                    0x65,
            SystemDynamicTimeZoneInformation =                      0x66,
            SystemCodeIntegrityInformation =                        0x67,
            SystemProcessorMicrocodeUpdateInformation =             0x68,
            SystemProcessorBrandString =                            0x69,
            SystemVirtualAddressInformation =                       0x6A,
            SystemLogicalProcessorAndGroupInformation =             0x6B,
            SystemProcessorCycleTimeInformation =                   0x6C,
            SystemStoreInformation =                                0x6D,
            SystemRegistryAppendString =                            0x6E,
            SystemAitSamplingValue =                                0x6F,
            SystemVhdBootInformation =                              0x70,
            SystemCpuQuotaInformation =                             0x71,
            SystemNativeBasicInformation =                          0x72,
            SystemErrorPortTimeouts =                               0x73,
            SystemLowPriorityIoInformation =                        0x74,
            SystemBootEntropyInformation =                          0x75,
            SystemVerifierCountersInformation =                     0x76,
            SystemPagedPoolInformationEx =                          0x77,
            SystemSystemPtesInformationEx =                         0x78,
            SystemNodeDistanceInformation =                         0x79,
            SystemAcpiAuditInformation =                            0x7A,
            SystemBasicPerformanceInformation =                     0x7B,
            SystemQueryPerformanceCounterInformation =              0x7C,
            SystemSessionBigPoolInformation =                       0x7D,
            SystemBootGraphicsInformation =                         0x7E,
            SystemScrubPhysicalMemoryInformation =                  0x7F,
            SystemBadPageInformation =                              0x80,
            SystemProcessorProfileControlArea =                     0x81,
            SystemCombinePhysicalMemoryInformation =                0x82,
            SystemEntropyInterruptTimingInformation =               0x83,
            SystemConsoleInformation =                              0x84,
            SystemPlatformBinaryInformation =                       0x85,
            SystemPolicyInformation =                               0x86,
            SystemHypervisorProcessorCountInformation =             0x87,
            SystemDeviceDataInformation =                           0x88,
            SystemDeviceDataEnumerationInformation =                0x89,
            SystemMemoryTopologyInformation =                       0x8A,
            SystemMemoryChannelInformation =                        0x8B,
            SystemBootLogoInformation =                             0x8C,
            SystemProcessorPerformanceInformationEx =               0x8D,
            SystemCriticalProcessErrorLogInformation =              0x8E,
            SystemSecureBootPolicyInformation =                     0x8F,
            SystemPageFileInformationEx =                           0x90,
            SystemSecureBootInformation =                           0x91,
            SystemEntropyInterruptTimingRawInformation =            0x92,
            SystemPortableWorkspaceEfiLauncherInformation =         0x93,
            SystemFullProcessInformation =                          0x94,
            SystemKernelDebuggerInformationEx =                     0x95,
            SystemBootMetadataInformation =                         0x96,
            SystemSoftRebootInformation =                           0x97,
            SystemElamCertificateInformation =                      0x98,
            SystemOfflineDumpConfigInformation =                    0x99,
            SystemProcessorFeaturesInformation =                    0x9A,
            SystemRegistryReconciliationInformation =               0x9B,
            SystemEdidInformation =                                 0x9C,
            SystemManufacturingInformation =                        0x9D,
            SystemEnergyEstimationConfigInformation =               0x9E,
            SystemHypervisorDetailInformation =                     0x9F,
            SystemProcessorCycleStatsInformation =                  0xA0,
            SystemVmGenerationCountInformation =                    0xA1,
            SystemTrustedPlatformModuleInformation =                0xA2,
            SystemKernelDebuggerFlags =                             0xA3,
            SystemCodeIntegrityPolicyInformation =                  0xA4,
            SystemIsolatedUserModeInformation =                     0xA5,
            SystemHardwareSecurityTestInterfaceResultsInformation = 0xA6,
            SystemSingleModuleInformation =                         0xA7,
            SystemAllowedCpuSetsInformation =                       0xA8,
            SystemDmaProtectionInformation =                        0xA9,
            SystemInterruptCpuSetsInformation =                     0xAA,
            SystemSecureBootPolicyFullInformation =                 0xAB,
            SystemCodeIntegrityPolicyFullInformation =              0xAC,
            SystemAffinitizedInterruptProcessorInformation =        0xAD,
            SystemRootSiloInformation =                             0xAE,
            SystemCpuSetInformation =                               0xAF,
            SystemCpuSetTagInformation =                            0xB0,
            SystemWin32WerStartCallout =                            0xB1,
            SystemSecureKernelProfileInformation =                  0xB2,
            SystemCodeIntegrityPlatformManifestInformation =        0xB3,
            SystemInterruptSteeringInformation =                    0xB4,
            SystemSuppportedProcessorArchitectures =                0xB5,
            SystemMemoryUsageInformation =                          0xB6,
            SystemCodeIntegrityCertificateInformation =             0xB7,
            SystemPhysicalMemoryInformation =                       0xB8,
            SystemControlFlowTransition =                           0xB9,
            SystemKernelDebuggingAllowed =                          0xBA,
            SystemActivityModerationExeState =                      0xBB,
            SystemActivityModerationUserSettings =                  0xBC,
            SystemCodeIntegrityPoliciesFullInformation =            0xBD,
            SystemCodeIntegrityUnlockInformation =                  0xBE,
            SystemIntegrityQuotaInformation =                       0xBF,
            SystemFlushInformation =                                0xC0,
            SystemProcessorIdleMaskInformation =                    0xC1,
            SystemSecureDumpEncryptionInformation =                 0xC2,
            SystemWriteConstraintInformation =                      0xC3,
            SystemKernelVaShadowInformation =                       0xC4,
            SystemHypervisorSharedPageInformation =                 0xC5,
            SystemFirmwareBootPerformanceInformation =              0xC6,
            SystemCodeIntegrityVerificationInformation =            0xC7,
            SystemFirmwarePartitionInformation =                    0xC8,
            SystemSpeculationControlInformation =                   0xC9,
            SystemDmaGuardPolicyInformation =                       0xCA,
            SystemEnclaveLaunchControlInformation =                 0xCB,
            SystemWorkloadAllowedCpuSetsInformation =               0xCC,
            SystemCodeIntegrityUnlockModeInformation =              0xCD,
            SystemLeapSecondInformation =                           0xCE,
            SystemFlags2Information =                               0xCF,
            SystemSecurityModelInformation =                        0xD0,
            SystemCodeIntegritySyntheticCacheInformation =          0xD1,
            MaxSystemInfoClass =                                    0xD2
        }

        //https://www.pinvoke.net/default.aspx/Enums.OBJECT_INFORMATION_CLASS
        protected enum OBJECT_INFORMATION_CLASS
        {
            ObjectBasicInformation =    0,
            ObjectNameInformation =     1,
            ObjectTypeInformation =     2,
            ObjectAllTypesInformation = 3,
            ObjectHandleInformation =   4
        }

        //https://docs.microsoft.com/en-za/windows/win32/procthread/process-security-and-access-rights
        //https://www.pinvoke.net/default.aspx/Enums.ProcessAccess
        protected enum PROCESS_ACCESS_RIGHTS
        {
            PROCESS_TERMINATE =                 0x00000001,
            PROCESS_CREATE_THREAD =             0x00000002,
            PROCESS_SET_SESSION_ID =            0x00000004,
            PROCESS_VM_OPERATION =              0x00000008,
            PROCESS_VM_READ =                   0x00000010,
            PROCESS_VM_WRITE =                  0x00000020,
            PROCESS_DUP_HANDLE =                0x00000040,
            PROCESS_CREATE_PROCESS =            0x00000080,
            PROCESS_SET_QUOTA =                 0x00000100,
            PROCESS_SET_INFORMATION =           0x00000200,
            PROCESS_QUERY_INFORMATION =         0x00000400,
            PROCESS_SUSPEND_RESUME =            0x00000800,
            PROCESS_QUERY_LIMITED_INFORMATION = 0x00001000,
            DELETE =                            0x00010000,
            READ_CONTROL =                      0x00020000,
            WRITE_DAC =                         0x00040000,
            WRITE_OWNER =                       0x00080000,
            STANDARD_RIGHTS_REQUIRED =          0x000F0000,
            SYNCHRONIZE =                       0x00100000,

            PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFFF
        }

        //https://docs.microsoft.com/en-us/windows/win32/api/handleapi/nf-handleapi-duplicatehandle#DUPLICATE_CLOSE_SOURCE
        protected enum DUPLICATE_HANDLE_OPTIONS
        {
            DUPLICATE_CLOSE_SOURCE = 0x00000001,
            DUPLICATE_SAME_ACCESS =  0x00000002
        }

        //http://www.jasinskionline.com/TechnicalWiki/SYSTEM_HANDLE_INFORMATION-WinApi-Struct.ashx
        internal enum SYSTEM_HANDLE_FLAGS : byte
        {
            PROTECT_FROM_CLOSE = 0x01,
            INHERIT =            0x02
        }

        //https://www.winehq.org/pipermail/wine-patches/2005-October/021642.html
        //https://github.com/olimsaidov/autorun-remover/blob/b558df6487ae1cb4cb998fab3330c07bb7de0f21/NativeAPI.pas#L108
        internal enum SYSTEM_HANDLE_TYPE
        {
            OB_TYPE_UNKNOWN =        00,
            OB_TYPE_TYPE =           01,
            OB_TYPE_DIRECTORY =      02,
            OB_TYPE_SYMBOLIC_LINK =  03,
            OB_TYPE_TOKEN =          04,
            OB_TYPE_PROCESS =        05,
            OB_TYPE_THREAD =         06,
            OB_TYPE_JOB =            07,
            OB_TYPE_EVENT =          08,
            OB_TYPE_EVENT_PAIR =     09,
            OB_TYPE_MUTANT =         10,
            OB_TYPE_UNKNOWN_11 =     11,
            OB_TYPE_SEMAPHORE =      12,
            OB_TYPE_TIMER =          13,
            OB_TYPE_PROFILE =        14,
            OB_TYPE_WINDOW_STATION = 15,
            OB_TYPE_DESKTOP =        16,
            OB_TYPE_SECTION =        17,
            OB_TYPE_KEY =            18,
            OB_TYPE_PORT =           19,
            OB_TYPE_WAITABLE_PORT =  20,
            OB_TYPE_ADAPTER =        21,
            OB_TYPE_CONTROLLER =     22,
            OB_TYPE_DEVICE =         23,
            OB_TYPE_DRIVER =         24,
            OB_TYPE_IO_COMPLETION =  25,
            OB_TYPE_FILE =           28,

        }

        #endregion

        #region Structs

        //https://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=18975&zep=OpenedFileFinder%2fUtils.h&rzp=%2fKB%2fshell%2fOpenedFileFinder%2f%2fopenedfilefinder_src.zip
        [StructLayout(LayoutKind.Sequential)]
        protected struct SYSTEM_HANDLE_INFORMATION
        {
            //public IntPtr dwCount;
            public uint dwCount;
            
            // see https://stackoverflow.com/a/38884095/2999220 - MarshalAs doesn't allow variable sized arrays
            //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct)]
            //public SYSTEM_HANDLE[] Handles;
            public IntPtr Handles;
        }

        //https://stackoverflow.com/a/5163277/2999220
        //http://www.jasinskionline.com/TechnicalWiki/SYSTEM_HANDLE_INFORMATION-WinApi-Struct.ashx
        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEM_HANDLE
        {
            public uint                dwProcessId;
            public byte                bObjectType;
            public SYSTEM_HANDLE_FLAGS bFlags;
            public ushort              wValue;
                   IntPtr              pAddress;
            public uint                GrantedAccess;
        }

        //https://docs.microsoft.com/en-us/windows/win32/api/ntdef/ns-ntdef-_unicode_string
        //https://www.pinvoke.net/default.aspx/Structures/UNICODE_STRING.html
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        protected struct UNICODE_STRING : IDisposable
        {
            public ushort Length;
            public ushort MaximumLength;
            //[MarshalAs(UnmanagedType.LPWStr)]
            private IntPtr Buffer;

            public UNICODE_STRING(string s)
            {
                Length = (ushort)(s.Length * 2);
                MaximumLength = (ushort)(Length + 2);
                Buffer = Marshal.StringToHGlobalUni(s);
            }

            public void Dispose()
            {
                Marshal.FreeHGlobal(Buffer);
                Buffer = IntPtr.Zero;
            }

            public override string ToString()
            {
                return Marshal.PtrToStringUni(Buffer);
            }
        }

        //https://www.pinvoke.net/default.aspx/Structures.GENERIC_MAPPING
        //http://www.jasinskionline.com/technicalwiki/GENERIC_MAPPING-WinApi-Struct.ashx
        [StructLayout(LayoutKind.Sequential)]
        protected struct GENERIC_MAPPING
        {
            public uint GenericRead;
            public uint GenericWrite;
            public uint GenericExecute;
            public uint GenericAll;
        }

        //http://www.jasinskionline.com/technicalwiki/OBJECT_NAME_INFORMATION-WinApi-Struct.ashx
        [StructLayout(LayoutKind.Sequential)]
        protected struct OBJECT_NAME_INFORMATION
        {
            public UNICODE_STRING Name;
        }

        //https://docs.microsoft.com/en-za/windows-hardware/drivers/ddi/ntifs/ns-ntifs-__public_object_type_information
        //http://www.jasinskionline.com/technicalwiki/OBJECT_TYPE_INFORMATION-WinApi-Struct.ashx
        [StructLayout(LayoutKind.Sequential)]
        protected struct OBJECT_TYPE_INFORMATION
        {
            public UNICODE_STRING   TypeName;
            public int              ObjectCount;
            public int              HandleCount;
            int                     Reserved1;
            int                     Reserved2;
            int                     Reserved3;
            int                     Reserved4;
            public int              PeakObjectCount;
            public int              PeakHandleCount;
            int                     Reserved5;
            int                     Reserved6;
            int                     Reserved7;
            int                     Reserved8;
            public int              InvalidAttributes;
            public GENERIC_MAPPING  GenericMapping;
            public int              ValidAccess;
            byte                    Unknown;
            public byte             MaintainHandleDatabase;
            public int              PoolType;
            public int              PagedPoolUsage;
            public int              NonPagedPoolUsage;
        }

        #endregion

        #region Methods

        //https://docs.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntquerysysteminformation
        [DllImport("ntdll.dll")]
        protected static extern NTSTATUS NtQuerySystemInformation(
            [In]  SYSTEM_INFORMATION_CLASS SystemInformationClass,
            [Out] IntPtr                   SystemInformation,
            [In]  uint                     SystemInformationLength,
            [Out] out uint                 ReturnLength
        );

        //https://docs.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntqueryobject
        [DllImport("ntdll.dll")]
        protected static extern NTSTATUS NtQueryObject(
            [In]  IntPtr                   Handle,
            [In]  OBJECT_INFORMATION_CLASS ObjectInformationClass,
            [In]  IntPtr                   ObjectInformation,
            [In]  uint                     ObjectInformationLength,
            [Out] out UIntPtr              ReturnLength
        );

        //https://docs.microsoft.com/en-za/windows/win32/api/processthreadsapi/nf-processthreadsapi-openprocess
        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern IntPtr OpenProcess(
            [In]                                PROCESS_ACCESS_RIGHTS dwDesiredAccess,
            [In, MarshalAs(UnmanagedType.Bool)] bool                  bInheritHandle,
            [In]                                uint                  dwProcessId
        );

        //https://docs.microsoft.com/en-us/windows/win32/api/handleapi/nf-handleapi-duplicatehandle
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        protected static extern bool DuplicateHandle(
            [In]                                IntPtr                   hSourceProcessHandle,
            [In]                                IntPtr                   hSourceHandle,
            [In]                                IntPtr                   hTargetProcessHandle,
            [Out]                               out IntPtr               lpTargetHandle,
            [In]                                uint                     dwDesiredAccess,
            [In, MarshalAs(UnmanagedType.Bool)] bool                     bInheritHandle,
            [In]                                DUPLICATE_HANDLE_OPTIONS dwOptions
        );

        //https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getcurrentprocess
        [DllImport("kernel32.dll")]
        protected static extern IntPtr GetCurrentProcess();

        //https://docs.microsoft.com/en-us/windows/win32/api/handleapi/nf-handleapi-closehandle
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        protected static extern bool CloseHandle(
            [In] IntPtr hObject
        );

        //https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-querydosdevicea
        //https://docs.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-querydosdevicew
        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern uint QueryDosDevice(
            [In]  string        lpDeviceName,
            [Out] StringBuilder lpTargetPath,
            [In]  uint          ucchMax
        );

        #endregion

        #endregion

        #region Public Methods

        internal static IEnumerable<SYSTEM_HANDLE> GetSystemHandles()
        {
            uint length = 0x1000;
            IntPtr ptr = IntPtr.Zero;
            bool done = false;
            try
            {
                while (!done)
                {
                    ptr = Marshal.AllocHGlobal((int)length);
                    uint wantedLength;
                    switch (NtQuerySystemInformation(
                        SYSTEM_INFORMATION_CLASS.SystemHandleInformation,
                        ptr, length, out wantedLength))
                    {
                        case NTSTATUS.STATUS_SUCCESS:
                            done = true;
                            break;
                        case NTSTATUS.STATUS_INFO_LENGTH_MISMATCH:
                            length = Math.Max(length, wantedLength);
                            Marshal.FreeHGlobal(ptr);
                            ptr = IntPtr.Zero;
                            break;
                        default:
                            throw new Exception("Failed to retrieve system handle information.", new Win32Exception());
                    }
                }

                int handleCount = IntPtr.Size == 4 ? Marshal.ReadInt32(ptr) : (int)Marshal.ReadInt64(ptr);
                int offset = IntPtr.Size;
                int size = Marshal.SizeOf(typeof(SYSTEM_HANDLE));

                for (int i = 0; i < handleCount; i++)
                {
                    SYSTEM_HANDLE struc = Marshal.PtrToStructure<SYSTEM_HANDLE>((IntPtr)((int)ptr + offset));
                    yield return struc;

                    offset += size;
                }
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }

        internal struct HandleInfo
        {
            public uint ProcessID;
            public ushort HandleID;
            public uint GrantedAccess;
            public byte RawType;
            public SYSTEM_HANDLE_FLAGS Flags;
            public string Name;
            public string TypeString;
            public SYSTEM_HANDLE_TYPE Type;
        }

        internal static HandleInfo GetHandleInfo(SYSTEM_HANDLE handle)
        {
            HandleInfo handleInfo = new HandleInfo
            {
                ProcessID = handle.dwProcessId,
                HandleID = handle.wValue,
                GrantedAccess = handle.GrantedAccess,
                RawType = handle.bObjectType,
                Flags = handle.bFlags
            };

            return handleInfo;
        }

        #endregion

    }
}
