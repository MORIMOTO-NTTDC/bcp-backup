const appVersion = "1.00";

const currentComponentName = ko.observable("SC00");

function setComponentName(name) {
    currentComponentName(name);
}

const global = {
    uvo: {
        vendorId: ko.observable(""),
        vendorName: ko.observable(""),
        parentId: ko.observable(""),
        userId: ko.observable(""),
        userName: ko.observable(""),
        userType: ko.observable(""),
        menuType: ko.observable(0),
        IsAdminUser: ko.observable(false),
        sc14mode: ko.observable(0),
    },
    const: {
        message: {
            i00001: "登録しました。",
            i00002: "更新しました。",
            i00003: "削除しました。",
            i00004: "%1を変更しました。",
            i00005: "%1します。よろしいですか？",

            i00011: "ログアウトしました。ログイン画面に遷移します。",
            i00012: "パスワードを初期化しました。初期化後のパスワードはメールで確認してください。",
            i00013: "アップロード実行をOFFにします。\n自動でファイルのアップロードが行われなくなります。\n本当によろしいですか？",
            i00014: "ダウンロード実行をONにします。\n端末側のフォルダの中身が、前回バックアップ時に置き換わります。\n本当によろしいですか？",
            i00015: "アップロード実行をOFFにされました。\n自動でファイルのアップロードが行われなくなります。\n本当によろしいですか？",
            i00016: "ダウンロード実行をONにされました。\n端末側のフォルダの中身が、前回バックアップ時に置き換わります。\n本当によろしいですか？",

            e00001: "予期しないエラーが発生しました。",
            e00002: "ログインが必要です。ログイン画面に遷移します。",
            e00003: "実行権限がありません。",
            e00004: "不正なリクエストです。",
            e00005: "入力エラーがあります。",

            e00010: "%1 は入力必須です。",
            e00011: "%1 には数字を入力してください。",
            e00012: "%1 には数値を入力してください。",
            e00013: "%1 には半角英数字で入力してください。",
            e00014: "%1 には半角英数字（記号含む）で入力してください。",
            e00015: "%1 に入力禁止文字\'%2\'が含まれています。",
            e00016: "%1 には、%2種類以上の文字種別を含めてください。",
            e00017: "%1 には%2文字以上で入力してください。",
            e00018: "%1 にはｅメールアドレスの形式で入力してください。",
            e00019: "%1 には正しい日付を入力してください。",
            e00020: "%1 には%2から%3までの範囲で入力してください。",
            e00021: "%1 には%2以降の日付を入力してください。",

            e00030: "ユーザID、メールアドレスまたはパスワードが間違っています。",
            e00031: "入力したメールアドレスは登録されていないため、パスワードを初期化できません。",
            e00032: "%1 と %2 の値が異なっています。同じ値を入力してください。",
            e00033: "%1 には %2 と異なる値を入力してください。",

            e00034: "%1 は見つかりませんでした。",
            e00035: "この%1はすでに使用されています。別の%2を入力してください。",
            e00036: "別のユーザ等により、すでに削除されているため、%1できません。",
            e00037: "別のユーザ等により、すでに更新されているため、%1できません。",
            e00038: "%1には%2 を入力してください。",
            e00039: "%1が間違っています。正しい%1を入力してください。",
            e00040: "バックアップ容量は使用容量より大きい値（GB）を選択してください。",

        },
        code: {
            userType: {
                sysAdmin: "0",      // システム管理者
                vendorAdmin: "1",   // ベンダ管理者
                vendorStaff: "2",   // ベンダ担当者
            },
            adminAuthority: {
                yes: "1",           // 管理者権限あり
                no: "0",            // なし
            },
            checkStatus: {
                valid: "1",         // 有効
                invalid: "0",       // 無効
            },
			displayStatus : {
				'0': '未実行',
				'1': 'ＵＬ成功',
				'2': 'ＤＬ成功',
				'3': 'ＵＬ失敗',
				'4': 'ＤＬ失敗',
				'5': '同期不可',
			},
			displayStatusLong : {
				'0': '未実行',
				'1': 'アップロード成功',
				'2': 'ダウンロード成功',
				'3': 'アップロード失敗',
				'4': 'ダウンロード失敗',
				'5': '同期不可',
			},
        }
    }
};

const menuList = [
	[
		// システム管理者用
	    {
	        name: "ホーム",
	        componentName: "SC11",
	    },{
	        name: "お知らせ管理",
	        componentName: "SC12",
	    },{
	        name: "ユーザ管理",
	        componentName: "SC13",
	    },{
	        name: "アカウント管理",
	        componentName: "SC14",
	    },{
	        name: "外部リンク",
	        componentName: "SC20",
	    }
	],
	[
		// ベンダ管理者用
	    {
	        name: "ホーム",
	        componentName: "SC11",
	    },{
	        name: "ユーザ管理",
	        componentName: "SC13",
	    },{
	        name: "アカウント管理",
	        componentName: "SC14",
	    },{
	        name: "外部リンク",
	        componentName: "SC20",
	    }
	],
	[
		// ベンダ担当者用
	    {
	        name: "ホーム",
	        componentName: "SC11",
	    },{
	        name: "アカウント管理",
	        componentName: "SC14",
	    },{
	        name: "外部リンク",
	        componentName: "SC20",
	    }
	]
];

function ajaxSetting() {
    $.ajaxSetup({
        type: "post",           // 全てPOSTメソッドで統一
        headers: {
            "X-Requested-With": "XMLHttpRequest"
        },
        dataType: 'json',       // レスポンスは全てJSON形式
        cache: false,           // キャッシュしない
        timeout: 1000 * 30,     // タイムアウト30秒
        dataType: "json",
        contentType: "application/json",
        dataFilter: function(data) {
            if (!data) {
                return null;
            }
            try {
                // 全てのプロパティの型をStringに統一する
                function castJsonPropertiesToString(obj) {
                    for (var key in obj) {
                        if (obj[key] instanceof Array || obj[key] instanceof Object) {
                            castJsonPropertiesToString(obj[key]);
                        } else {
                            if (obj[key] === null) {
                                obj[key] = '';
                            } else {
                                obj[key] = String(obj[key]);
                            }
                        }
                    }
                    return obj;
                }
                json = castJsonPropertiesToString(JSON.parse(data));
                return JSON.stringify(json);
            } catch (e) {
                return data;
            }
        }
    });

    // ajax global events
    $(document)
        .ajaxSend(function () { $('#loading').show();$('#loading>img').show(); })
        .ajaxStop(function () {
            $('#loading>img').hide();
            setTimeout(function() {
                $('#loading').hide();
            }, 100);
        });
}

// routing
var routingList = [
    { hash: "SC00", component: "SC00", loginCheck: false},
    { hash: "SC02", component: "SC02", loginCheck: true},
    { hash: "SC11", component: "SC11", loginCheck: true},
    { hash: "SC12", component: "SC12", loginCheck: true},
    { hash: "SC13", component: "SC13", loginCheck: true},
    { hash: "SC14", component: "SC14", loginCheck: true},
    { hash: "SC20", component: "SC20", loginCheck: true},
];

// Determine the component name from url hash.
function routing() {

    var hashName = location.hash.split("/")[1];

    // ex. index.html
    if (hashName === undefined) {
        currentComponentName("SC00");
        return;
    };

    var routings = routingList.filter((root) => {
        if (hashName.split("@").length > 1) {
            // ex. index.html#/SC21@1
            return root.hash === hashName.split("@")[0];
        }
        return root.hash === hashName;
    });

    if (routings.length === 0) {
        currentComponentName("SC11");
        return;
    }

    // When page was reloaded, fetch UVO from server.
    if (global.uvo.userId() === "" && routings[0].loginCheck === true) {

        // ユーザ情報を取得するまでの間は、空白ページを表示しておく
        currentComponentName("BLANK");

        $.ajax({
            url: "./userinfo.do",
            headers: {"X-XSRF-TOKEN": getCsrfTokenFromCookie()},
        }).done((data) => {
            global.uvo.vendorId(data.vendorId);
            global.uvo.parentId(data.parentId);
            global.uvo.vendorName(data.vendorName);
            global.uvo.userId(data.userId);
            global.uvo.userName(data.userName);
            global.uvo.userType(data.userType);
            global.uvo.menuType(parseInt(data.userType, 10));
            global.uvo.IsAdminUser((data.userType === global.const.code.userType.sysAdmin) ? true : ((data.userType === global.const.code.userType.vendorAdmin) ? true : false));
            currentComponentName(routings[0].component);
        }).fail((xhr) => {
            if (xhr.status === 401) {
                alert(global.const.message.e00003);
                location.hash = "/SC00";
            } else {
                alert(global.const.message.e00001 + " http status:" + xhr.status);
            }
        });
    } else {
        currentComponentName(routings[0].component);
    }
}

window.onhashchange = function () {
    routing();
};

ajaxSetting();

routing();

ko.applyBindings({});

// input check
// 必須チェック（message.e00010）
function checkRequire(target) {
    if (target === null || typeof target === 'undefined'){
        return false;
    }
    return target.trim().length > 0;
}
// 整数チェック（message.e00011）
function isNumber(target) {
    if (target === null || target.length === 0) {
        return true;
    }
    if (!target.match(/^([1-9]\d*|0)$/)) {
        return false;
    }
    return true;
}
// 数値チェック（message.e00012）
function isNumeric(target) {
    if (target === null || target.length === 0) {
        return true;
    }
    if (!target.match(/^[-]?([1-9]\d*|0)(\.\d+)?$/)) {
        return false;
    }
    return true;
}
// 英数チェック（message.e00013）
function alphaNumeric(target) {
    if (target === null || target.length === 0) {
        return true;
    }
    if (target.match(/[^a-zA-Z0-9]+/)) {
        return false;
    }
    return true;
}
// 半角英数記号（禁止文字除く）チェック（message.e00014）
function alphaNumericSymbol(target) {
    if (target === null || target.length === 0) {
        return true;
    }
    // 半角スペースは除く半角文字
    if (!target.match(/^[\x20-\x7e]+$/)){
        return false;
    }
    var ngChars = /[\\/:*?"<>|]/.exec(target);
    if (ngChars !== null) {
        return false;
    }
    return true;
}
// 有効文字チェック（message.e00015）
// 入力禁止文字があれば、その文字を返却。なければ空文字
function ngCharCode(target) {
    if (target === null || target.length === 0) {
        return '';
    }
    var str2array = function (str) {
        var array = [], i, il = str.length;
        for (i = 0; i < il; i++) {
            array.push(str.charCodeAt(i));
        }
        return array;
    };
    const target2 = Array.from(target);
    for (var i = 0; i < target2.length; i++) {
        var c = target2[i], array = str2array(c);
        let sjis_array;
        try {
	        sjis_array = Encoding.convert(array, {to:'SJIS', from:'UNICODE', fallback: 'error'});
        } catch(e) {
        	return c;
        }
        buf = sjis_array[0].toString(16);
        if (sjis_array.length > 1) {
            buf = buf + sjis_array[1].toString(16);
        }
        var sjis_code = parseInt(buf, 16);
        if ((0xed40 <= sjis_code && sjis_code <= 0xeefc) || (0xfa40 <= sjis_code && sjis_code <= 0xfc4b) || (0xf040 <= sjis_code && sjis_code <= 0xf9fc) || (0x8540 <= sjis_code && sjis_code <= 0x889e) || (0xeaa5 <= sjis_code && sjis_code <= 0xfcfc)) {
            // 入力禁止文字を返却
            return c;
        }
    }
    return '';
}
// 入力禁止文字チェック（message.e00015）
// 入力禁止文字があれば、その文字を返却。なければ空文字
function ngChar(target) {
    if (target === null || target.length === 0) {
        return '';
    }
    // 環境依存文字チェック
    var c = ngCharCode(target);
    if (c) {
        // 入力禁止文字を返却
        return c;
    }
    var ngChars = /[\\/:*?"<>|]/.exec(target);
    if (ngChars !== null) {
        // 入力禁止文字を返却
        return ngChars[0];
    }
    return '';
}
// 半角英（a～z,A～Z）数（0～9）記号混在チェック（message.e00016）
function mixAlphaNumericSymbol(target, words) {
    var cnt = 0;
    if (target.match(/[0-9]/)) {
        cnt ++;
    }
    if (target.match(/[a-z]/)) {
        cnt ++;
    }
    if (target.match(/[A-Z]/)) {
        cnt ++;
    }
    const reg = new RegExp(/[!#$%&'()\+\-\.,;=@\[\]^_`{}~]/g);
    if (reg.test(target)) {
        cnt ++;
    }
    if (cnt < words) {
        return false;
    }
    return true;
}
// 最小文字数チェック（message.e00017）
function minLength(target, words) {
    if (target === null || target.length === 0) {
        return true;
    }
    if (target.length < parseInt(words, 10)) {
        return false;
    }
    return true;
}
// eメールアドレス形式チェック（message.e00018）
function isEmailAddress(target) {
    if (target === null || target.length === 0) {
        return true;
    }
    if(!target.match(/.+@.+\..+/)){
        return false;
    }
    return true;
}
// 日付チェック（message.e00019）
function isDate(target) {
    if (target === null || target.length === 0) {
        return true;
    }
    var date = new Date(target);
    if (isNaN(date.getDate())) {
        return false;
    }
    return true;
}
// ファイルパスチェック（message.e00020）
// 下記の禁止文字をチェック
// ⁄	スラッシュ	パスの区切り記号
// *	アスタリスク	ワイルドカード記号
// ?	疑問符	ワイルドカード記号
// "	2重引用符	パスを囲む
// >	大なり不等号	リダイレクト記号
// <	小なり不等号	リダイレクト記号
// |	縦棒、バー	パイプ記号
// ※　\（円マーク）はパスの区切り記号で使用するため許可
// ※　:（コロン）はドライブ文字記号で使用するため許可
function isFilePath(target) {
    if (target === null || target.length === 0) {
        return '';
    }
    var ngChars = /[\/*?"<>|]/.exec(target);
    if (ngChars !== null) {
        // 入力禁止文字を返却
        return ngChars[0];
    }
    return '';
}
// 改行を<BR>に変換
function newLinesToBR(text) {
    const escapedText = text.replace(/</g, '&lt;').replace(/>/g, '&gt;');
    return escapedText.replace(/\r?\n/g, '<br>');
};

// datepicker
ko.bindingHandlers.datepicker = {
	init: function (element, valueAccessor, allBindingsAccessor) {
		var options = allBindingsAccessor().datepickerOptions || {}, $el = $(element);
		options.autoclose = true;
		options.format = 'yyyy/mm/dd';
		options.language = 'ja';
		options.forceParse = false;
		options.keyboardNavigation = false;
		options.todayHighlight = true;
		$el.datepicker(options);
		ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
			$el.datepicker('destroy');
		});
	},
	 update: function (element, valueAccessor, allBindingsAccessor) {
        $(element).datepicker('setDate', ko.unwrap(valueAccessor()));
	}
};

function flash_TR_element(elem) {
    $(elem).children().addClass("flash").addClass("hightlight");
    setTimeout(function() {
        $(elem).children().removeClass("hightlight");
    }, 50);
}

function flash_CARD_element(elem) {
    $(elem).children().addClass("flash").addClass("hightlight");
    setTimeout(function() {
        $(elem).children().removeClass("hightlight");
    }, 50);
}

const smartPhone = ko.observable(false);

function isSmartPhone() {
    if (document.body.clientWidth < 576) {
        smartPhone(true);
    } else {
        smartPhone(false);
    }
}
$(window).resize(function() {
    isSmartPhone();
});
isSmartPhone();

// cookieから、Spring Security の XSRF-TOKEN の文字列を取り出す
function getCsrfTokenFromCookie() {
    const token = document.cookie
        .split(/;\s*/)
        .find(row => {
            [key, value] = row.split('=')
            return key === 'XSRF-TOKEN';
        });
    if (token) {
        return token.split('=')[1];
    }
    return "";
}
