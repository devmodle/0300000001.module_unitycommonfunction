using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using System.IO;
using MessagePack;

/** 함수 */
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
	public static void WriteBytes(string a_oFilePath, byte[] a_oBytes, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsAssert = true) {
		CAccess.Assert(!a_bIsAssert || (a_oBytes != null && a_oFilePath.ExIsValid()));

		// 기록이 가능 할 경우
		if(a_oBytes != null && a_oFilePath.ExIsValid()) {
			using(var oWStream = CAccess.GetWriteStream(a_oFilePath)) {
				CFunc.WriteBytes(oWStream, a_oBytes, a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default, a_bIsAssert);
			}
		}
	}

	/** 바이트를 기록한다 */
	public static void WriteBytes(FileStream a_oWStream, byte[] a_oBytes, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsAssert = true) {
		CAccess.Assert(!a_bIsAssert || (a_oWStream != null && a_oBytes != null));

		// 스트림이 존재 할 경우
		if(a_oWStream != null && a_oBytes != null) {
			string oBase64Str = System.Convert.ToBase64String(a_oBytes, KCDefine.B_VAL_0_INT, a_oBytes.Length);
			a_oWStream.Write(a_bIsBase64 ? (a_oEncoding ?? System.Text.Encoding.Default).GetBytes(oBase64Str) : a_oBytes);

			a_oWStream.Flush(true);
		}
	}

	/** 문자열을 기록한다 */
	public static void WriteStr(string a_oFilePath, 
		string a_oStr, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsAssert = true) {

		CAccess.Assert(!a_bIsAssert || (a_oStr != null && a_oFilePath.ExIsValid()));

		// 기록이 가능 할 경우
		if(a_oStr != null && a_oFilePath.ExIsValid()) {
			using(var oWStream = CAccess.GetWriteStream(a_oFilePath)) {
				CFunc.WriteStr(oWStream, a_oStr, a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default, a_bIsAssert);
			}
		}
	}

	/** 문자열을 기록한다 */
	public static void WriteStr(FileStream a_oWStream, 
		string a_oStr, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsAssert = true) {

		CAccess.Assert(!a_bIsAssert || (a_oWStream != null && a_oStr != null));

		// 스트림이 존재 할 경우
		if(a_oWStream != null && a_oStr != null) {
			var oEncoding = a_oEncoding ?? System.Text.Encoding.Default;
			CFunc.WriteBytes(a_oWStream, oEncoding.GetBytes(a_oStr), a_bIsBase64, oEncoding, a_bIsAssert);
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
	public static void Invoke<TA>(ref System.Action<TA> a_oAction, TA a_tParamsA) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB>(ref System.Action<TA, TB> a_oAction, TA a_tParamsA, TB a_tParamsB) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC>(ref System.Action<TA, TB, TC> a_oAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC, TD>(ref System.Action<TA, TB, TC, TD> a_oAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC, TD, TE>(ref System.Action<TA, TB, TC, TD, TE> a_oAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC, TD, TE, TF>(ref System.Action<TA, TB, TC, TD, TE, TF> a_oAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC, TD, TE, TF, TG>(ref System.Action<TA, TB, TC, TD, TE, TF, TG> a_oAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF, TG a_tParamsG) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF, a_tParamsG);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC, TD, TE, TF, TG, TH>(ref System.Action<TA, TB, TC, TD, TE, TF, TG, TH> a_oAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF, TG a_tParamsG, TH a_tParamsH) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF, a_tParamsG, a_tParamsH);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC, TD, TE, TF, TG, TH, TI>(ref System.Action<TA, TB, TC, TD, TE, TF, TG, TH, TI> a_oAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF, TG a_tParamsG, TH a_tParamsH, TI a_tParamsI) {
		var oAction = a_oAction;

		try {
			a_oAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF, a_tParamsG, a_tParamsH, a_tParamsI);
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
	public static Result Invoke<TA, Result>(ref System.Func<TA, Result> a_oFunc, TA a_tParamsA) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, Result>(ref System.Func<TA, TB, Result> a_oFunc, TA a_tParamsA, TB a_tParamsB) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, Result>(ref System.Func<TA, TB, TC, Result> a_oFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, TD, Result>(ref System.Func<TA, TB, TC, TD, Result> a_oFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, TD, TE, Result>(ref System.Func<TA, TB, TC, TD, TE, Result> a_oFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, TD, TE, TF, Result>(ref System.Func<TA, TB, TC, TD, TE, TF, Result> a_oFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, TD, TE, TF, TG, Result>(ref System.Func<TA, TB, TC, TD, TE, TF, TG, Result> a_oFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF, TG a_tParamsG) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF, a_tParamsG);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, TD, TE, TF, TG, TH, Result>(ref System.Func<TA, TB, TC, TD, TE, TF, TG, TH, Result> a_oFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF, TG a_tParamsG, TH a_tParamsH) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF, a_tParamsG, a_tParamsH);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, TD, TE, TF, TG, TH, TI, Result>(ref System.Func<TA, TB, TC, TD, TE, TF, TG, TH, TI, Result> a_oFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF, TG a_tParamsG, TH a_tParamsH, TI a_tParamsI) {
		CAccess.Assert(a_oFunc != null);
		var oFunc = a_oFunc;

		try {
			a_oFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF, a_tParamsG, a_tParamsH, a_tParamsI);
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
	public static void WriteMsgPackObj<T>(string a_oFilePath, T a_oObj, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsAssert = true) {
		CAccess.Assert(!a_bIsAssert || a_oFilePath.ExIsValid());

		// 경로가 유효 할 경우
		if(a_oFilePath.ExIsValid()) {
			CFunc.WriteBytes(a_oFilePath, MessagePackSerializer.Serialize<T>(a_oObj), a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default, a_bIsAssert);
		}
	}

	/** 메세지 팩 JSON 객체를 기록한다 */
	public static void WriteMsgPackJSONObj<T>(string a_oFilePath, T a_oObj, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsAssert = true) {
		CAccess.Assert(!a_bIsAssert || a_oFilePath.ExIsValid());

		// 경로가 유효 할 경우
		if(a_oFilePath.ExIsValid()) {
			CFunc.WriteStr(a_oFilePath, a_oObj.ExToMsgPackJSONStr(), a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default, a_bIsAssert);
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
	public static void WriteJSONObj<T>(string a_oFilePath, T a_oObj, bool a_bIsBase64, System.Text.Encoding a_oEncoding = null, bool a_bIsNeedsRoot = false, bool a_bIsPretty = false, bool a_bIsAssert = true) {
		CAccess.Assert(!a_bIsAssert || a_oFilePath.ExIsValid());

		// 경로가 유효 할 경우
		if(a_oFilePath.ExIsValid()) {
			CFunc.WriteStr(a_oFilePath, a_oObj.ExToJSONStr(a_bIsNeedsRoot, a_bIsPretty), a_bIsBase64, a_oEncoding ?? System.Text.Encoding.Default, a_bIsAssert);
		}
	}
#endif // #if NEWTON_SOFT_JSON_SERIALIZE_DESERIALIZE_ENABLE
	#endregion // 조건부 제네릭 클래스 함수
}
