//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: public.proto
namespace @public
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MsgSession")]
  public partial class MsgSession : global::ProtoBuf.IExtensible
  {
    public MsgSession() {}
    

    private ulong _pid = default(ulong);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"pid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(ulong))]
    public ulong pid
    {
      get { return _pid; }
      set { _pid = value; }
    }

    private uint _roomcode = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"roomcode", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint roomcode
    {
      get { return _roomcode; }
      set { _roomcode = value; }
    }

    private string _actionCode = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"actionCode", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string actionCode
    {
      get { return _actionCode; }
      set { _actionCode = value; }
    }

    private string _callbackCode = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"callbackCode", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string callbackCode
    {
      get { return _callbackCode; }
      set { _callbackCode = value; }
    }

    private string _serverName = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"serverName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string serverName
    {
      get { return _serverName; }
      set { _serverName = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MsgLogin")]
  public partial class MsgLogin : global::ProtoBuf.IExtensible
  {
    public MsgLogin() {}
    

    private string _token = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"token", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string token
    {
      get { return _token; }
      set { _token = value; }
    }

    private string _appId = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"appId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string appId
    {
      get { return _appId; }
      set { _appId = value; }
    }

    private string _hostId = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"hostId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string hostId
    {
      get { return _hostId; }
      set { _hostId = value; }
    }

    private string _channelId = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"channelId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string channelId
    {
      get { return _channelId; }
      set { _channelId = value; }
    }

    private string _uuid = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"uuid", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string uuid
    {
      get { return _uuid; }
      set { _uuid = value; }
    }

    private ulong _pid = default(ulong);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"pid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(ulong))]
    public ulong pid
    {
      get { return _pid; }
      set { _pid = value; }
    }

    private ulong _time = default(ulong);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(ulong))]
    public ulong time
    {
      get { return _time; }
      set { _time = value; }
    }

    private string _sig = "";
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"sig", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string sig
    {
      get { return _sig; }
      set { _sig = value; }
    }

    private string _ipAds = "";
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"ipAds", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string ipAds
    {
      get { return _ipAds; }
      set { _ipAds = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MsgOnLine")]
  public partial class MsgOnLine : global::ProtoBuf.IExtensible
  {
    public MsgOnLine() {}
    

    private ulong _player_id = default(ulong);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"player_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(ulong))]
    public ulong player_id
    {
      get { return _player_id; }
      set { _player_id = value; }
    }

    private uint _roomCode = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"roomCode", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint roomCode
    {
      get { return _roomCode; }
      set { _roomCode = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MsgOffLine")]
  public partial class MsgOffLine : global::ProtoBuf.IExtensible
  {
    public MsgOffLine() {}
    

    private uint _roomCode = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"roomCode", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint roomCode
    {
      get { return _roomCode; }
      set { _roomCode = value; }
    }

    private ulong _player_id = default(ulong);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"player_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(ulong))]
    public ulong player_id
    {
      get { return _player_id; }
      set { _player_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}