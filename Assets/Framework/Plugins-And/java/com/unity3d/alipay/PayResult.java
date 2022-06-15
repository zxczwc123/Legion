package com.unity3d.alipay;

import android.text.TextUtils;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.Map;

public class PayResult {
	private String resultStatus;
	private String result;
	private String memo;

	public PayResult(Map<String, String> rawResult) {
		if (rawResult == null) {
			return;
		}

		for (String key : rawResult.keySet()) {
			if (TextUtils.equals(key, "resultStatus")) {
				resultStatus = rawResult.get(key);
			} else if (TextUtils.equals(key, "result")) {
				result = rawResult.get(key);
			} else if (TextUtils.equals(key, "memo")) {
				memo = rawResult.get(key);
			}
		}
	}

	@Override
	public String toString() {
		JSONObject jsonObject = new JSONObject();
		try {
			jsonObject.put("resultStatus",resultStatus);
			jsonObject.put("memo",memo);
			jsonObject.put("result",result);
			return jsonObject.toString();
		} catch (JSONException e) {
			e.printStackTrace();
			return null;
		}
	}

	/**
	 * @return the resultStatus
	 */
	public String getResultStatus() {
		return resultStatus;
	}

	/**
	 * @return the memo
	 */
	public String getMemo() {
		return memo;
	}

	/**
	 * @return the result
	 */
	public String getResult() {
		return result;
	}
}
