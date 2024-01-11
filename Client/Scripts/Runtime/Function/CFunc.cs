using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using System.IO;
using MessagePack;

/** 기본 함수 */
public static partial class CFunc {
	#region 클래스 함수
	/** 바이트를 읽어들인다 */
	public static byte[] ReadBytes(string a_oFilePath, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());

		// 파일이 존재 할 경우
		if(File.Exists(a_oFilePath)) {
			var oBytes = File.ReadAllBytes(a_oFilePath);
			return a_bIsBase64 ? System.Convert.FromBase64String((a_oEncoding ?? System.Text.Encoding.Default).GetString(oBytes)) : oBytes;
		}

		return null;
	}

	/** 바이트를 읽어들인다 */
	public static byte[] ReadBytes(Stream a_oStream, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oStream != null);
		var oBytes = new byte[a_oStream.Length];

		a_oStream.Read(oBytes);
		return a_bIsBase64 ? System.Convert.FromBase64String((a_oEncoding ?? System.Text.Encoding.Default).GetString(oBytes)) : oBytes;
	}

	/** 바이트를 읽어들인다 */
	public static byte[] ReadBytesFromRes(string a_oFilePath, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oTextAsset = Resources.Load<TextAsset>(a_oFilePath);

		// 텍스트 에셋이 존재 할 경우
		if(oTextAsset.ExIsValid()) {
			return a_bIsBase64 ? System.Convert.FromBase64String((a_oEncoding ?? System.Text.Encoding.Default).GetString(oTextAsset.bytes)) : oTextAsset.bytes;
		}

		return null;
	}

	/** 문자열을 읽어들인다 */
	public static string ReadStr(string a_oFilePath, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());

		// 파일이 존재 할 경우
		if(File.Exists(a_oFilePath)) {
			var oBytes = CFunc.ReadBytes(a_oFilePath, a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default);
			return a_bIsBase64 ? (a_oEncoding ?? System.Text.Encoding.Default).GetString(oBytes) : File.ReadAllText(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default);
		}

		return string.Empty;
	}

	/** 문자열을 읽어들인다 */
	public static string ReadStr(Stream a_oStream, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oStream != null);
		var oBytes = CFunc.ReadBytes(a_oStream, a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default);

		return (a_oEncoding ?? System.Text.Encoding.Default).GetString(oBytes);
	}

	/** 문자열을 읽어들인다 */
	public static string ReadStrFromRes(string a_oFilePath, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oTextAsset = Resources.Load<TextAsset>(a_oFilePath);

		// 텍스트 에셋이 존재 할 경우
		if(oTextAsset.ExIsValid()) {
			var oBytes = CFunc.ReadBytesFromRes(a_oFilePath, a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default);
			return a_bIsBase64 ? (a_oEncoding ?? System.Text.Encoding.Default).GetString(oBytes) : oTextAsset.text;
		}

		return string.Empty;
	}

	/** 문자열 라인을 읽어들인다 */
	public static string[] ReadStrLines(string a_oFilePath, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return File.Exists(a_oFilePath) ? File.ReadAllLines(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default) : null;
	}

	/** 바이트를 기록한다 */
	public static void WriteBytes(string a_oFilePath, byte[] a_oBytes, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oBytes != null && a_oFilePath.ExIsValid()));

		// 기록이 가능 할 경우
		if(a_oBytes != null && a_oFilePath.ExIsValid()) {
			using(var oWStream = CAccess.GetWriteStream(a_oFilePath)) {
				CFunc.WriteBytes(oWStream, a_oBytes, a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default, a_bIsEnableAssert);
			}
		}
	}

	/** 바이트를 기록한다 */
	public static void WriteBytes(FileStream a_oWStream, byte[] a_oBytes, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oWStream != null && a_oBytes != null));

		// 스트림이 존재 할 경우
		if(a_oWStream != null && a_oBytes != null) {
			string oBase64Str = System.Convert.ToBase64String(a_oBytes, KCDefine.B_VAL_0_INT, a_oBytes.Length);
			a_oWStream.Write(a_bIsBase64 ? (a_oEncoding ?? System.Text.Encoding.Default).GetBytes(oBase64Str) : a_oBytes);

			a_oWStream.Flush(true);
		}
	}

	/** 문자열을 기록한다 */
	public static void WriteStr(string a_oFilePath, 
		string a_oStr, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsEnableAssert = true) {

		CAccess.Assert(!a_bIsEnableAssert || (a_oStr != null && a_oFilePath.ExIsValid()));

		// 기록이 가능 할 경우
		if(a_oStr != null && a_oFilePath.ExIsValid()) {
			using(var oWStream = CAccess.GetWriteStream(a_oFilePath)) {
				CFunc.WriteStr(oWStream, a_oStr, a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default, a_bIsEnableAssert);
			}
		}
	}

	/** 문자열을 기록한다 */
	public static void WriteStr(FileStream a_oWStream, 
		string a_oStr, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsEnableAssert = true) {

		CAccess.Assert(!a_bIsEnableAssert || (a_oWStream != null && a_oStr != null));

		// 스트림이 존재 할 경우
		if(a_oWStream != null && a_oStr != null) {
			var oEncoding = a_oEncoding ?? System.Text.Encoding.Default;
			CFunc.WriteBytes(a_oWStream, oEncoding.GetBytes(a_oStr), a_bIsBase64, oEncoding, a_bIsEnableAssert);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke(ref System.Action a_oAction) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke();
		}
	}
	#endregion // 클래스 함수

	#region 제네릭 클래스 함수
	/** 함수를 호출한다 */
	public static void Invoke<T01>(ref System.Action<T01> a_oAction, T01 a_tParams01) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParams01);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<T01, T02>(ref System.Action<T01, T02> a_oAction, T01 a_tParams01, T02 a_tParams02) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParams01, a_tParams02);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<T01, T02, T03>(ref System.Action<T01, T02, T03> a_oAction, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParams01, a_tParams02, a_tParams03);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<T01, T02, T03, T04>(ref System.Action<T01, T02, T03, T04> a_oAction, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03, T04 a_tParams04) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParams01, a_tParams02, a_tParams03, a_tParams04);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<T01, T02, T03, T04, T05>(ref System.Action<T01, T02, T03, T04, T05> a_oAction, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03, T04 a_tParams04, T05 a_tParams05) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParams01, a_tParams02, a_tParams03, a_tParams04, a_tParams05);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<T01, T02, T03, T04, T05, T06>(ref System.Action<T01, T02, T03, T04, T05, T06> a_oAction, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03, T04 a_tParams04, T05 a_tParams05, T06 a_tParams06) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParams01, a_tParams02, a_tParams03, a_tParams04, a_tParams05, a_tParams06);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<T01, T02, T03, T04, T05, T06, T07>(ref System.Action<T01, T02, T03, T04, T05, T06, T07> a_oAction, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03, T04 a_tParams04, T05 a_tParams05, T06 a_tParams06, T07 a_tParams07) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParams01, a_tParams02, a_tParams03, a_tParams04, a_tParams05, a_tParams06, a_tParams07);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<T01, T02, T03, T04, T05, T06, T07, T08>(ref System.Action<T01, T02, T03, T04, T05, T06, T07, T08> a_oAction, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03, T04 a_tParams04, T05 a_tParams05, T06 a_tParams06, T07 a_tParams07, T08 a_tParams08) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParams01, a_tParams02, a_tParams03, a_tParams04, a_tParams05, a_tParams06, a_tParams07, a_tParams08);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<T01, T02, T03, T04, T05, T06, T07, T08, T09>(ref System.Action<T01, T02, T03, T04, T05, T06, T07, T08, T09> a_oAction, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03, T04 a_tParams04, T05 a_tParams05, T06 a_tParams06, T07 a_tParams07, T08 a_tParams08, T09 a_tParams09) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParams01, a_tParams02, a_tParams03, a_tParams04, a_tParams05, a_tParams06, a_tParams07, a_tParams08, a_tParams09);
		}
	}

	/** 함수를 호출한다 */
	public static Result Invoke<Result>(ref System.Func<Result> a_oFunc) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke();
	}

	/** 함수를 호출한다 */
	public static Result Invoke<T01, Result>(ref System.Func<T01, Result> a_oFunc, T01 a_tParams01) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParams01);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<T01, T02, Result>(ref System.Func<T01, T02, Result> a_oFunc, T01 a_tParams01, T02 a_tParams02) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParams01, a_tParams02);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<T01, T02, T03, Result>(ref System.Func<T01, T02, T03, Result> a_oFunc, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParams01, a_tParams02, a_tParams03);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<T01, T02, T03, T04, Result>(ref System.Func<T01, T02, T03, T04, Result> a_oFunc, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03, T04 a_tParams04) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParams01, a_tParams02, a_tParams03, a_tParams04);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<T01, T02, T03, T04, T05, Result>(ref System.Func<T01, T02, T03, T04, T05, Result> a_oFunc, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03, T04 a_tParams04, T05 a_tParams05) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParams01, a_tParams02, a_tParams03, a_tParams04, a_tParams05);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<T01, T02, T03, T04, T05, T06, Result>(ref System.Func<T01, T02, T03, T04, T05, T06, Result> a_oFunc, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03, T04 a_tParams04, T05 a_tParams05, T06 a_tParams06) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParams01, a_tParams02, a_tParams03, a_tParams04, a_tParams05, a_tParams06);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<T01, T02, T03, T04, T05, T06, T07, Result>(ref System.Func<T01, T02, T03, T04, T05, T06, T07, Result> a_oFunc, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03, T04 a_tParams04, T05 a_tParams05, T06 a_tParams06, T07 a_tParams07) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParams01, a_tParams02, a_tParams03, a_tParams04, a_tParams05, a_tParams06, a_tParams07);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<T01, T02, T03, T04, T05, T06, T07, T08, Result>(ref System.Func<T01, T02, T03, T04, T05, T06, T07, T08, Result> a_oFunc, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03, T04 a_tParams04, T05 a_tParams05, T06 a_tParams06, T07 a_tParams07, T08 a_tParams08) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParams01, a_tParams02, a_tParams03, a_tParams04, a_tParams05, a_tParams06, a_tParams07, a_tParams08);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<T01, T02, T03, T04, T05, T06, T07, T08, T09, Result>(ref System.Func<T01, T02, T03, T04, T05, T06, T07, T08, T09, Result> a_oFunc, T01 a_tParams01, T02 a_tParams02, T03 a_tParams03, T04 a_tParams04, T05 a_tParams05, T06 a_tParams06, T07 a_tParams07, T08 a_tParams08, T09 a_tParams09) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParams01, a_tParams02, a_tParams03, a_tParams04, a_tParams05, a_tParams06, a_tParams07, a_tParams08, a_tParams09);
	}

	/** 메세지 팩 객체를 읽어들인다 */
	public static T ReadMsgPackObj<T>(string a_oFilePath, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return MessagePackSerializer.Deserialize<T>(CFunc.ReadBytes(a_oFilePath, a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default));
	}

	/** 메세지 팩 객체를 읽어들인다 */
	public static T ReadMsgPackObjFromRes<T>(string a_oFilePath, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return MessagePackSerializer.Deserialize<T>(CFunc.ReadBytesFromRes(a_oFilePath, a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default));
	}

	/** 메세지 팩 JSON 객체를 읽어들인다 */
	public static T ReadMsgPackJSONObj<T>(string a_oFilePath, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return CFunc.ReadStr(a_oFilePath, a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default).ExMsgPackJSONStrToObj<T>();
	}

	/** 메세지 팩 JSON 객체를 읽어들인다 */
	public static T ReadMsgPackJSONObjFromRes<T>(string a_oFilePath, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return CFunc.ReadStrFromRes(a_oFilePath, a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default).ExMsgPackJSONStrToObj<T>();
	}

	/** 메세지 팩 객체를 기록한다 */
	public static void WriteMsgPackObj<T>(string a_oFilePath, T a_oObj, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || a_oFilePath.ExIsValid());

		// 경로가 유효 할 경우
		if(a_oFilePath.ExIsValid()) {
			CFunc.WriteBytes(a_oFilePath, MessagePackSerializer.Serialize<T>(a_oObj), a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default, a_bIsEnableAssert);
		}
	}

	/** 메세지 팩 JSON 객체를 기록한다 */
	public static void WriteMsgPackJSONObj<T>(string a_oFilePath, T a_oObj, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || a_oFilePath.ExIsValid());

		// 경로가 유효 할 경우
		if(a_oFilePath.ExIsValid()) {
			CFunc.WriteStr(a_oFilePath, a_oObj.ExToMsgPackJSONStr(), a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default, a_bIsEnableAssert);
		}
	}
	#endregion // 제네릭 클래스 함수

	#region 조건부 제네릭 클래스 함수
#if NEWTON_SOFT_JSON_SERIALIZE_DESERIALIZE_ENABLE
	/** JSON 객체를 읽어들인다 */
	public static T ReadJSONObj<T>(string a_oFilePath, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return CFunc.ReadStr(a_oFilePath, a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default).ExJSONStrToObj<T>();
	}

	/** JSON 객체를 읽어들인다 */
	public static T ReadJSONObjFromRes<T>(string a_oFilePath, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return CFunc.ReadStrFromRes(a_oFilePath, a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default).ExJSONStrToObj<T>();
	}

	/** JSON 객체를 기록한다 */
	public static void WriteJSONObj<T>(string a_oFilePath, T a_oObj, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsNeedsRoot = false, bool a_bIsPretty = false, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || a_oFilePath.ExIsValid());

		// 경로가 유효 할 경우
		if(a_oFilePath.ExIsValid()) {
			CFunc.WriteStr(a_oFilePath, a_oObj.ExToJSONStr(a_bIsNeedsRoot, a_bIsPretty), a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default, a_bIsEnableAssert);
		}
	}
#endif // #if NEWTON_SOFT_JSON_SERIALIZE_DESERIALIZE_ENABLE
	#endregion // 조건부 제네릭 클래스 함수
}
