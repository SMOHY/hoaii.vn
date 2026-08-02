// Điều khiển Google Website Translator qua cookie `googtrans`.
//
// Widget đọc cookie này lúc tải trang rồi tự dịch DOM, nên đổi ngôn ngữ
// = ghi cookie + reload. Không có API phía máy chủ nào tham gia.
(function () {
    "use strict";

    var COOKIE = "googtrans";
    var PAGE_LANG = "vi";
    var ONE_YEAR = 31536000;

    function readCookie(name) {
        var parts = document.cookie.split(";");
        for (var i = 0; i < parts.length; i++) {
            var part = parts[i].trim();
            if (part.indexOf(name + "=") === 0) {
                return decodeURIComponent(part.substring(name.length + 1));
            }
        }
        return "";
    }

    // Google lưu cặp ngôn ngữ dạng "/vi/en". Kết thúc bằng /en nghĩa là
    // trang đang hiển thị tiếng Anh.
    function currentLang() {
        var raw = readCookie(COOKIE);
        return raw.slice(-3) === "/en" ? "en" : "vi";
    }

    // Phải ghi ở mọi phạm vi domain widget có thể đọc, nếu không widget và
    // trang sẽ bất đồng về ngôn ngữ đang bật. Trên localhost hai dòng
    // domain= bị trình duyệt bỏ qua — vô hại, dòng đầu vẫn có tác dụng.
    function writeCookie(value, maxAge) {
        var host = location.hostname;
        var scopes = ["", ";domain=" + host, ";domain=." + host];
        for (var i = 0; i < scopes.length; i++) {
            document.cookie = COOKIE + "=" + value + ";path=/;max-age=" + maxAge + scopes[i];
        }
    }

    function setLang(lang) {
        if (lang === "en") {
            writeCookie("/" + PAGE_LANG + "/en", ONE_YEAR);
        } else {
            // Xoá hẳn cookie tốt hơn là ghi "/vi/vi": widget vẫn coi là đang
            // dịch và để lại thanh banner cũ.
            writeCookie("", 0);
        }
        location.reload();
    }

    // Làm mờ bên ĐANG KHÔNG được chọn, dùng lại đúng rule .nav-lang .en
    // { opacity: .5 } có sẵn trong nav.css nên không phải đụng CSS.
    function label(lang) {
        return lang === "en"
            ? "<span class=\"en\">VN</span>/EN"
            : "VN/<span class=\"en\">EN</span>";
    }

    function wire() {
        var lang = currentLang();
        var next = lang === "en" ? "vi" : "en";
        var toEnglish = next === "en";

        var btn = document.querySelector(".nav-lang");
        if (btn) {
            // Không cho widget dịch chính cái nút đổi ngôn ngữ, nếu không
            // "VN/EN" sẽ bị viết lại thành thứ khác.
            btn.classList.add("notranslate");
            btn.setAttribute("translate", "no");
            btn.innerHTML = label(lang);
            btn.setAttribute("aria-label", toEnglish ? "Switch to English" : "Chuyển sang tiếng Việt");
            btn.addEventListener("click", function () {
                setLang(next);
            });
        }

        // .nav-desktop bị ẩn dưới 768px nên trên điện thoại nút trên không
        // với tới được — trước giờ mobile không có chỗ nào đổi ngôn ngữ.
        var drawer = document.querySelector(".nav-drawer__links");
        if (drawer) {
            var link = document.createElement("a");
            // Mượn lại class của link anh em để khỏi phải đoán tên class
            // trong nav.css.
            var sibling = drawer.querySelector("a");
            if (sibling) {
                link.className = sibling.className;
            }
            link.classList.add("notranslate");
            link.setAttribute("translate", "no");
            link.href = "#";
            link.textContent = toEnglish ? "English" : "Tiếng Việt";
            link.addEventListener("click", function (event) {
                event.preventDefault();
                setLang(next);
            });
            drawer.appendChild(link);
        }
    }

    // element.js gọi ngược lại hàm này qua tham số ?cb=
    window.googleTranslateElementInit = function () {
        /* global google */
        new google.translate.TranslateElement({
            pageLanguage: PAGE_LANG,
            includedLanguages: "vi,en",
            // Tự bung sẽ hiện thanh banner của Google; ta tự làm nút riêng.
            autoDisplay: false
        }, "google_translate_element");
    };

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", wire);
    } else {
        wire();
    }
})();
