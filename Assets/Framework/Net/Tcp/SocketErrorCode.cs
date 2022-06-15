// ========================================================
// 描 述：SocketErrorCode.cs 
// 创 建：高辉 
// 时 间：2021/11/24 15:37:36 
// 版 本：2020.3.18f1c1 
// ========================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.SocketNet {
    
    public enum ErrorCode {
        // 自定义类型
        SendError = 20001,
        RecvError = 20002,
        PeerDisconnect = 20003,
        ConnectSuccess = 20004,
        Disconnect = 20005,
        AlreadyHadSocket = 20006,
        ParseError = 20007,
        GetError = 20008,
        ListenError = 20009,
        // Socket类型
        SocketError = -1, // 0xFFFFFFFF
        Success = 0,
        OperationAborted = 995, // 0x000003E3
        IOPending = 997, // 0x000003E5
        Interrupted = 10004, // 0x00002714
        AccessDenied = 10013, // 0x0000271D
        Fault = 10014, // 0x0000271E
        InvalidArgument = 10022, // 0x00002726
        TooManyOpenSockets = 10024, // 0x00002728
        WouldBlock = 10035, // 0x00002733
        InProgress = 10036, // 0x00002734
        AlreadyInProgress = 10037, // 0x00002735
        NotSocket = 10038, // 0x00002736
        DestinationAddressRequired = 10039, // 0x00002737
        MessageSize = 10040, // 0x00002738
        ProtocolType = 10041, // 0x00002739
        ProtocolOption = 10042, // 0x0000273A
        ProtocolNotSupported = 10043, // 0x0000273B
        SocketNotSupported = 10044, // 0x0000273C
        OperationNotSupported = 10045, // 0x0000273D
        ProtocolFamilyNotSupported = 10046, // 0x0000273E
        AddressFamilyNotSupported = 10047, // 0x0000273F
        AddressAlreadyInUse = 10048, // 0x00002740
        AddressNotAvailable = 10049, // 0x00002741
        NetworkDown = 10050, // 0x00002742
        NetworkUnreachable = 10051, // 0x00002743
        NetworkReset = 10052, // 0x00002744
        ConnectionAborted = 10053, // 0x00002745
        ConnectionReset = 10054, // 0x00002746
        IsConnected = 10056, // 0x00002748
        NotConnected = 10057, // 0x00002749
        Shutdown = 10058, // 0x0000274A
        TimedOut = 10060, // 0x0000274C
        ConnectionRefused = 10061, // 0x0000274D
        HostDown = 10064, // 0x00002750
        HostUnreachable = 10065, // 0x00002751
        ProcessLimit = 10067, // 0x00002753
        SystemNotReady = 10091, // 0x0000276B
        VersionNotSupported = 10092, // 0x0000276C
        NotInitialized = 10093, // 0x0000276D
        Disconnecting = 10101, // 0x00002775
        TypeNotFound = 10109, // 0x0000277D
        HostNotFound = 11001, // 0x00002AF9
        TryAgain = 11002, // 0x00002AFA
        NoRecovery = 11003, // 0x00002AFB
        NoData = 11004, // 0x00002AFC
    }
}
