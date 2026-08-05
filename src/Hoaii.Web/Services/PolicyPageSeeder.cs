using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Services;

/// <summary>
/// Seeds the storefront policy pages into the database. Runs on every startup but only inserts
/// pages whose slug doesn't exist yet, so new pages added here later reach an already-seeded
/// database without touching content an admin may have edited. The copy for the original four
/// (trao-doi/giao-hang/dieu-khoan-su-dung/bao-mat) is transcribed from Figma (nodes 1246:42533,
/// 1246:43442, 1246:43583, 1246:43724); the rest is compliance copy for the legally required
/// storefront disclosures (pricing/payment, complaint handling, owner-info footer link).
/// </summary>
public static class PolicyPageSeeder
{
    private static PolicyBlock P(string t) => new() { Kind = PolicyBlockKind.Paragraph, Text = t };
    private static PolicyBlock H(string t) => new() { Kind = PolicyBlockKind.Heading, Text = t };
    private static PolicyBlock B(string t) => new() { Kind = PolicyBlockKind.Bullet, Text = t };

    public static async Task EnsureSeedAsync(HoaiiDbContext db)
    {
        var existingSlugs = await db.PolicyPages.Select(p => p.Slug).ToListAsync();

        var pages = new List<PolicyPage>
        {
            new()
            {
                Slug = "trao-doi",
                Title = "CHÍNH SÁCH ĐỔI TRẢ & HOÀN TÁC",
                NavLabel = "Chính sách trao đổi",
                BreadcrumbLabel = "Trang chủ/Chính sách trao đổi",
                SortOrder = 1,
                Blocks =
                [
                    P("Tại HOÀI, mỗi sản phẩm được trao đi là một nhân duyên lành. Chúng tôi trân quý sự tin tưởng của bạn và luôn mong muốn mang lại trải nghiệm thoải mái nhất. Trong trường hợp món đồ nhận được chưa thực sự như ý, HOÀI sẵn lòng đồng hành cùng bạn để tìm phương án vẹn toàn."),
                    H("I. Điều Kiện Đổi Trả Sản Phẩm"),
                    P("Để đảm bảo quyền lợi, Quý Khách vui lòng kiểm tra kỹ tình trạng sản phẩm ngay tại thời điểm nhận hàng. HOÀI hỗ trợ đổi trả ngay nếu sản phẩm gặp các vấn đề sau:"),
                    B("Sự cố về vận chuyển: Bao bì bị rách hỏng, sản phẩm bị bong tróc, nứt vỡ hoặc biến dạng do va đập."),
                    B("Sự cố về đóng gói: Sản phẩm không đúng mẫu mã, chủng loại như đơn hàng đã đặt; hoặc bị thiếu hụt số lượng, phụ kiện và quà tặng đi kèm."),
                    P("Lưu ý nhỏ: Quý khách vui lòng lưu lại hình ảnh và video mở hộp (unboxing) làm minh chứng để HOÀI có thể hỗ trợ xử lý thủ tục đổi trả một cách nhanh chóng và chính xác nhất."),
                    H("II. Quy Định Thời Gian & Phương Thức Gửi Trả"),
                    B("Thời gian thông báo: Trong vòng 48 giờ kể từ khi ký nhận hàng (đối với các trường hợp thiếu hụt hoặc hư hỏng vật lý)."),
                    B("Thời gian gửi hoàn sản phẩm: Trong vòng 14 ngày kể từ ngày nhận hàng thành công."),
                    B("Phương thức gửi trả: Bạn có thể mang sản phẩm ghé chơi và đổi trực tiếp tại cửa hàng/văn phòng của HOÀI, hoặc gửi chuyển phát qua bưu điện/các đơn vị vận chuyển thuận tiện nhất cho bạn."),
                    H("III. Chi Phí Vận Chuyển Hoàn Hàng"),
                    P("Tùy thuộc vào nguyên nhân phát sinh (do sơ suất của HOÀI hay nhu cầu cá nhân từ phía khách hàng), hai bên sẽ cùng trao đổi để thống nhất phương án hỗ trợ chi phí vận chuyển hợp lý và vẹn cả đôi đường."),
                    H("IV. Quy Trình Hoàn Tiền"),
                    P("Ngay sau khi nhận lại sản phẩm và hoàn tất việc kiểm tra tình trạng hàng hóa, HOÀI sẽ tiến hành hoàn trả tiền hàng cho bạn. Thời gian hoàn tiền được xử lý nhanh chóng trong vòng 48 giờ kể từ khi HOÀI xác nhận nhận lại hàng thành công."),
                    P("Trước khi gửi hoàn sản phẩm, bạn hãy liên hệ trước với HOÀI qua hotline hoặc hộp thư tin nhắn để HOÀI chuẩn bị và đón nhận kiện hàng một cách chu đáo nhất."),
                    P("Mọi ý kiến đóng góp hoặc phản hồi về chất lượng, HOÀI luôn lắng nghe tại đường dây chăm sóc khách hàng. Sự hài lòng của bạn chính là động lực để HOÀI hoàn thiện hơn mỗi ngày. Cảm ơn bạn đã thương mến!"),
                ],
            },
            new()
            {
                Slug = "giao-hang",
                Title = "CHÍNH SÁCH GIAO NHẬN HÀNG HÓA",
                NavLabel = "Chính sách giao hàng",
                BreadcrumbLabel = "Trang chủ/Chính sách giao hàng",
                SortOrder = 2,
                Blocks =
                [
                    P("Sau khi quý khách hoàn tất đặt hàng và thống nhất phương thức thanh toán trên website, HOÀI sẽ tiến hành bàn giao sản phẩm theo các hình thức vận chuyển dưới đây:"),
                    H("1. Các Phương Thức Vận Chuyển"),
                    B("Giao hàng tiêu chuẩn (Toàn quốc) — Thời gian dự kiến: 1-2 ngày đối với khu vực nội thành Hà Nội và TP. Hồ Chí Minh; Khu vực miền Bắc: 2-4 ngày; Khu vực miền Trung: 4-7 ngày; Khu vực miền Nam: 5-7 ngày. Chi phí thay đổi tùy theo khu vực nhận hàng (nhân viên điều phối của HOÀI sẽ liên hệ để thông báo chi phí chính xác trước khi gửi đi)."),
                    B("Giao hàng hỏa tốc — Phạm vi: Toàn quốc (đặc biệt tối ưu tại nội thành Hà Nội và TP. Hồ Chí Minh). Thời gian dự kiến: 1-4 giờ đối với nội thành (qua các đơn vị đối tác như Grab, AhaMove, Lalamove, Be...); 1-2 ngày đối với các tỉnh thành khác. Áp dụng biểu phí hỏa tốc của đơn vị vận chuyển và được thông báo trước đến quý khách."),
                    P("Lưu ý đối với đơn hàng gấp: Nếu có nhu cầu nhận hàng gấp trong ngày tại nội thành Hà Nội hoặc TP. Hồ Chí Minh, quý khách vui lòng liên hệ trực tiếp với HOÀI qua Fanpage Facebook hoặc Zalo để được ưu tiên xử lý kịp thời."),
                    H("2. Quy Định Trách Nhiệm Vận Chuyển"),
                    B("Trường hợp sử dụng dịch vụ do HOÀI chỉ định: HOÀI chịu trách nhiệm hoàn toàn về sự nguyên vẹn của hàng hóa trước các rủi ro (mất mát, hư hại) trong suốt quá trình vận chuyển, đảm bảo sản phẩm tới tay quý khách đúng chuẩn mực chất lượng."),
                    B("Trường hợp sử dụng dịch vụ do Quý khách chỉ định (tự gọi shipper/xe khách): Quý khách vui lòng hoàn tất thanh toán 100% giá trị đơn hàng trước khi HOÀI bàn giao sản phẩm cho đơn vị vận chuyển. HOÀI sẽ chụp ảnh/quay video xác nhận tình trạng nguyên vẹn của gói hàng trước khi giao. Trách nhiệm đối với sản phẩm sau khi rời khỏi kho của HOÀI sẽ thuộc về đơn vị vận chuyển do quý khách điều phối."),
                    H("3. Yêu Cầu Xuất Hóa Đơn"),
                    P("Trong trường hợp cần phát hành hóa đơn hoặc các chứng từ kế toán liên quan đến đơn hàng, quý khách vui lòng thông báo trước và cung cấp đầy đủ thông tin cho nhân viên hỗ trợ trong quá trình xác nhận vận chuyển."),
                ],
            },
            new()
            {
                Slug = "dieu-khoan-su-dung",
                Title = "ĐIỀU KHOẢN SỬ DỤNG",
                NavLabel = "Điều khoản sử dụng",
                BreadcrumbLabel = "Trang chủ/Điều khoản sử dụng",
                SortOrder = 3,
                Blocks =
                [
                    H("1. Quy Định Chung"),
                    P("Bằng việc truy cập và sử dụng website của HOÀI, quý khách đồng ý chấp thuận các điều khoản được nêu dưới đây."),
                    P("HOÀI có quyền cập nhật, điều chỉnh hoặc thay đổi bất kỳ nội dung nào trong văn bản này vào bất kỳ thời điểm nào. Các thay đổi sẽ có hiệu lực ngay sau khi được đăng tải chính thức trên website mà không cần thông báo trước. Việc quý khách tiếp tục sử dụng dịch vụ sau khi các cập nhật được công bố đồng nghĩa với việc chấp thuận các thay đổi đó. Vui lòng kiểm tra định kỳ để nắm rõ các quy định hiện hành."),
                    H("2. Tiêu Chuẩn Sử Dụng Website"),
                    P("Để thực hiện các giao dịch mua bán trên website của HOÀI, người sử dụng cần đảm bảo các điều kiện sau:"),
                    B("Đủ 18 tuổi hoặc truy cập dưới sự giám sát của cha mẹ/người giám hộ hợp pháp."),
                    B("Có đầy đủ năng lực hành vi dân sự theo quy định hiện hành của pháp luật Việt Nam."),
                    P("Về việc nhận thông tin: Trong quá trình đăng ký tài khoản hoặc mua hàng, quý khách mặc định đồng ý nhận các thông tin cập nhật hoặc thông báo từ HOÀI. Nếu không có nhu cầu tiếp tục nhận email, quý khách có thể chủ động từ chối bất kỳ lúc nào bằng cách nhấp vào liên kết “Hủy đăng ký” ở dưới cùng của mỗi email."),
                    H("3. Phương Thức Thanh Toán"),
                    P("Để tối ưu hóa trải nghiệm, quý khách có thể lựa chọn một trong các hình thức thanh toán phù hợp dưới đây:"),
                    B("Thanh toán trực tiếp: Nhận hàng và thanh toán tại hệ thống cửa hàng/văn phòng của HOÀI."),
                    B("Thanh toán khi nhận hàng (COD): Thanh toán bằng tiền mặt cho đơn vị vận chuyển ngay khi nhận sản phẩm."),
                    B("Thanh toán trực tuyến: Chuyển khoản ngân hàng hoặc sử dụng các cổng thanh toán/thẻ tín dụng được tích hợp trên website."),
                ],
            },
            new()
            {
                Slug = "bao-mat",
                Title = "CHÍNH SÁCH BẢO VỆ DỮ LIỆU CÁ NHÂN",
                NavLabel = "Chính sách bảo vệ dữ liệu cá nhân",
                BreadcrumbLabel = "Trang chủ/Chính sách bảo vệ dữ liệu cá nhân",
                SortOrder = 4,
                Blocks = BaoMatV2Blocks(),
            },
            new()
            {
                Slug = "gia-thanh-toan",
                Title = "CHÍNH SÁCH GIÁ & THANH TOÁN",
                NavLabel = "Chính sách giá & thanh toán",
                BreadcrumbLabel = "Trang chủ/Chính sách giá & thanh toán",
                SortOrder = 5,
                Blocks =
                [
                    H("I. Chính Sách Giá"),
                    B("Giá bán sản phẩm niêm yết trên website đã bao gồm thuế giá trị gia tăng (nếu có) và được thể hiện bằng đồng Việt Nam (VNĐ)."),
                    B("Giá có thể được điều chỉnh theo chương trình khuyến mãi hoặc biến động thị trường mà không cần báo trước; tuy nhiên đơn hàng đã được xác nhận sẽ giữ nguyên mức giá tại thời điểm đặt hàng."),
                    B("Trường hợp phát sinh sai lệch giá do lỗi kỹ thuật hiển thị, HOÀI sẽ chủ động liên hệ để thông báo và cùng khách hàng thống nhất phương án xử lý (tiếp tục giao dịch theo giá đúng hoặc hủy đơn, hoàn tiền nếu đã thanh toán) trước khi giao hàng."),
                    H("II. Phương Thức Thanh Toán"),
                    P("Quý khách có thể lựa chọn một trong các hình thức thanh toán sau khi đặt hàng trên website:"),
                    B("Thanh toán khi nhận hàng (COD)."),
                    B("Chuyển khoản ngân hàng — thông tin tài khoản cụ thể được cung cấp tại bước thanh toán hoặc theo yêu cầu qua hotline."),
                    B("Thanh toán trực tiếp tại cửa hàng/văn phòng của HOÀI."),
                    B("Thanh toán qua cổng thanh toán trực tuyến (nếu được kích hoạt trên website)."),
                    H("III. Xác Nhận Đơn Hàng & Hóa Đơn"),
                    P("Đơn hàng được xem là hoàn tất khi quý khách nhận được xác nhận từ HOÀI qua email, điện thoại hoặc tin nhắn. Trường hợp cần xuất hóa đơn hoặc chứng từ kế toán, quý khách vui lòng cung cấp đầy đủ thông tin trước khi đơn hàng được giao."),
                ],
            },
            new()
            {
                Slug = "khieu-nai",
                Title = "CƠ CHẾ TIẾP NHẬN & GIẢI QUYẾT PHẢN ÁNH, KHIẾU NẠI",
                NavLabel = "Giải quyết khiếu nại",
                BreadcrumbLabel = "Trang chủ/Giải quyết khiếu nại",
                SortOrder = 6,
                Blocks = KhieuNaiV2Blocks(),
            },
            new()
            {
                Slug = "quyen-va-nghia-vu",
                Title = "QUYỀN VÀ NGHĨA VỤ CỦA CÁC BÊN",
                NavLabel = "Quyền và nghĩa vụ các bên",
                BreadcrumbLabel = "Trang chủ/Quyền và nghĩa vụ các bên",
                SortOrder = 7,
                Blocks = QuyenNghiaVuBlocks(),
            },
            new()
            {
                Slug = "uu-tien-hien-thi",
                Title = "CHÍNH SÁCH VỀ ƯU TIÊN HIỂN THỊ",
                NavLabel = "Chính sách ưu tiên hiển thị",
                BreadcrumbLabel = "Trang chủ/Chính sách ưu tiên hiển thị",
                SortOrder = 8,
                Blocks = UuTienHienThiBlocks(),
            },
            new()
            {
                Slug = "dieu-kien-han-che",
                Title = "CÁC ĐIỀU KIỆN, HẠN CHẾ TRONG VIỆC CUNG CẤP HÀNG HÓA",
                NavLabel = "Điều kiện, hạn chế cung cấp hàng hóa",
                BreadcrumbLabel = "Trang chủ/Điều kiện, hạn chế cung cấp hàng hóa",
                SortOrder = 9,
                Blocks = DieuKienHanCheBlocks(),
            },
        };

        var newPages = pages.Where(p => !existingSlugs.Contains(p.Slug)).ToList();
        foreach (var page in newPages)
        {
            for (var i = 0; i < page.Blocks.Count; i++)
            {
                page.Blocks[i].SortOrder = i;
            }
        }

        if (newPages.Count > 0)
        {
            db.PolicyPages.AddRange(newPages);
            await db.SaveChangesAsync();
        }

        await EnsureWarrantySectionAsync(db);
        await ReplaceBlocksIfOutdatedAsync(db, "bao-mat", "Điều 5 Nghị định 248/2026", BaoMatV2Blocks);
        await ReplaceBlocksIfOutdatedAsync(db, "khieu-nai", "Điều 7 Nghị định 248/2026", KhieuNaiV2Blocks);
    }

    /// <summary>
    /// bao-mat and khieu-nai already existed in the DB before the TMĐT notification-dossier
    /// rewrite, so the add-if-missing logic above never touches their content. This replaces a
    /// page's blocks wholesale the first time it runs against a database whose content predates
    /// the given marker string (unique to the new copy) — safe to leave running forever since it
    /// no-ops once the marker is present.
    /// </summary>
    private static async Task ReplaceBlocksIfOutdatedAsync(HoaiiDbContext db, string slug, string marker, Func<List<PolicyBlock>> newBlocks)
    {
        var page = await db.PolicyPages
            .Include(p => p.Blocks)
            .FirstOrDefaultAsync(p => p.Slug == slug);
        if (page is null || page.Blocks.Any(b => b.Text.Contains(marker)))
        {
            return;
        }

        db.PolicyBlocks.RemoveRange(page.Blocks);
        var blocks = newBlocks();
        for (var i = 0; i < blocks.Count; i++)
        {
            blocks[i].SortOrder = i;
            blocks[i].PolicyPageId = page.Id;
        }
        db.PolicyBlocks.AddRange(blocks);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// The "trao-doi" (exchange/return) page originally had no warranty clause. Appends one the
    /// first time this runs against a database where the page already exists without it, so it
    /// reaches sites seeded before this section was written without duplicating on every restart.
    /// </summary>
    private static async Task EnsureWarrantySectionAsync(HoaiiDbContext db)
    {
        var page = await db.PolicyPages
            .Include(p => p.Blocks)
            .FirstOrDefaultAsync(p => p.Slug == "trao-doi");
        if (page is null || page.Blocks.Any(b => b.Text.Contains("Bảo Hành")))
        {
            return;
        }

        var nextOrder = page.Blocks.Count == 0 ? 0 : page.Blocks.Max(b => b.SortOrder) + 1;
        var warranty = new List<PolicyBlock>
        {
            H("V. Chính Sách Bảo Hành"),
            P("Đối với các sản phẩm phụ kiện đi kèm có tính chất sử dụng lâu dài (bình, hộp, khung, vật phẩm trang trí...), HOÀI hỗ trợ bảo hành lỗi do nhà sản xuất trong vòng 30 ngày kể từ ngày nhận hàng, áp dụng với các lỗi kỹ thuật/vật liệu không do quá trình sử dụng, va đập hoặc bảo quản không đúng cách của khách hàng gây ra."),
            P("Riêng đối với hoa tươi và các thành phần tươi sống trong giỏ/hộp quà, do đặc thù dễ hư hỏng theo thời gian, chính sách bảo hành không áp dụng; các vấn đề phát sinh ngay khi nhận hàng được xử lý theo Mục I và II ở trên."),
        };
        for (var i = 0; i < warranty.Count; i++)
        {
            warranty[i].SortOrder = nextOrder + i;
        }

        db.PolicyBlocks.AddRange(warranty.Select(b => { b.PolicyPageId = page.Id; return b; }));
        await db.SaveChangesAsync();
    }

    /// <summary>Điều 5 Nghị định 248/2026 — must cover all 8 items a–h or the TMĐT notification
    /// dossier gets bounced back for "insufficient content" (this happened once already).</summary>
    private static List<PolicyBlock> BaoMatV2Blocks() =>
    [
        P("Tại HOÀI, sự tin tưởng của bạn là điều chúng tôi trân trọng nhất. Chính sách này minh bạch cách HOÀI thu thập, sử dụng, lưu trữ và bảo vệ dữ liệu cá nhân của bạn, được xây dựng theo quy định của pháp luật Việt Nam về dữ liệu và bảo vệ dữ liệu cá nhân, bao gồm Luật Thương mại điện tử số 122/2025/QH15 và Nghị định 248/2026/NĐ-CP (Điều 5)."),

        H("1. Mục Đích Và Phạm Vi Thu Thập Thông Tin"),
        P("HOÀI thu thập thông tin của bạn nhằm:"),
        B("Xác nhận, xử lý và giao đơn hàng bạn đặt trên website;"),
        B("Liên hệ tư vấn, hỗ trợ và chăm sóc khách hàng, xử lý phản ánh, khiếu nại;"),
        B("Thực hiện dịch vụ cá nhân hóa sản phẩm (in khắc logo, thiết kế theo yêu cầu, gói quà theo yêu cầu);"),
        B("Thực hiện nghĩa vụ bảo hành, đổi trả, hoàn tiền theo chính sách đã công bố;"),
        B("Xuất hóa đơn và thực hiện nghĩa vụ kế toán, thuế theo quy định pháp luật;"),
        B("Gửi thông tin về sản phẩm mới, chương trình ưu đãi — chỉ khi bạn đã đăng ký nhận tin, và bạn có thể hủy đăng ký bất kỳ lúc nào;"),
        B("Cải thiện chất lượng sản phẩm, dịch vụ và trải nghiệm trên website."),
        P("Phạm vi thông tin thu thập: họ và tên, số điện thoại, thư điện tử, địa chỉ nhận hàng (bắt buộc); tên đơn vị/mã số thuế/địa chỉ xuất hóa đơn (khi có yêu cầu); nội dung cá nhân hóa như logo, tên riêng, thông điệp in trên sản phẩm (khi sử dụng dịch vụ cá nhân hóa); lịch sử đơn hàng, sản phẩm đã mua (tự động ghi nhận); địa chỉ IP, loại trình duyệt, thiết bị truy cập (tự động ghi nhận)."),
        P("HOÀI không thu thập số tài khoản ngân hàng, số thẻ hoặc mã bảo mật thẻ của bạn. Các thông tin này (nếu có) do tổ chức cung ứng dịch vụ thanh toán trực tiếp xử lý theo quy định bảo mật riêng của tổ chức đó."),

        H("2. Phạm Vi Sử Dụng Thông Tin"),
        P("HOÀI sử dụng thông tin đã thu thập trong phạm vi sau:"),
        B("Nội bộ HOÀI: các bộ phận bán hàng, chăm sóc khách hàng, thiết kế, kho vận, kế toán truy cập theo đúng chức năng được phân quyền;"),
        B("Đơn vị vận chuyển: chỉ gồm họ tên, số điện thoại, địa chỉ người nhận, phục vụ mục đích giao hàng;"),
        B("Tổ chức cung ứng dịch vụ thanh toán: thông tin giao dịch phục vụ xử lý thanh toán;"),
        B("Cơ quan nhà nước có thẩm quyền: khi có yêu cầu hợp pháp bằng văn bản."),
        P("HOÀI không mua bán, trao đổi, cho thuê hoặc chuyển giao dữ liệu cá nhân của bạn cho bên thứ ba vì mục đích thương mại khi chưa có sự đồng ý của bạn."),

        H("3. Thời Gian Lưu Trữ Thông Tin"),
        B("Thông tin tài khoản khách hàng: trong suốt thời gian tài khoản hoạt động và 12 tháng sau khi tài khoản bị hủy;"),
        B("Thông tin đơn hàng, hợp đồng giao kết: tối thiểu 03 năm kể từ thời điểm giao kết (theo Điều 16 Luật Thương mại điện tử);"),
        B("File thiết kế, logo khách hàng cung cấp: 24 tháng kể từ ngày hoàn tất đơn hàng, phục vụ đặt lại;"),
        B("Chứng từ kế toán, hóa đơn: theo pháp luật kế toán — tối thiểu 10 năm;"),
        B("Nhật ký truy cập kỹ thuật: 12 tháng."),
        P("Hết thời hạn nêu trên, dữ liệu được xóa hoặc ẩn danh hóa, trừ trường hợp pháp luật yêu cầu lưu trữ dài hơn."),

        H("4. Tổ Chức, Cá Nhân Có Thể Được Tiếp Cận Thông Tin Cá Nhân"),
        B("Nhân sự HOÀI được phân quyền — theo chức năng, nhiệm vụ được phân công;"),
        B("Đơn vị vận chuyển hợp tác — chỉ họ tên, số điện thoại, địa chỉ nhận hàng, phục vụ giao hàng;"),
        B("Tổ chức cung ứng dịch vụ thanh toán (nếu có) — thông tin giao dịch, phục vụ xử lý thanh toán;"),
        B("Đơn vị cung cấp hạ tầng lưu trữ (hosting) — dữ liệu lưu trên máy chủ, phục vụ vận hành kỹ thuật website;"),
        B("Cơ quan nhà nước có thẩm quyền — theo phạm vi yêu cầu, phục vụ thanh tra, kiểm tra, điều tra theo quy định."),

        H("5. Biện Pháp Bảo Mật Thông Tin, Dữ Liệu Của Người Sử Dụng"),
        B("Website sử dụng chứng chỉ bảo mật SSL/TLS, mã hóa dữ liệu trên đường truyền;"),
        B("Phân quyền truy cập theo vai trò — mỗi nhân sự chỉ tiếp cận dữ liệu trong phạm vi công việc;"),
        B("Mật khẩu tài khoản được lưu dưới dạng băm (hash) — HOÀI không lưu và không thể xem mật khẩu gốc của bạn;"),
        B("Sao lưu dữ liệu định kỳ hằng ngày;"),
        B("Ghi nhật ký truy cập hệ thống quản trị;"),
        B("Nhân sự tiếp xúc dữ liệu khách hàng được phổ biến quy định bảo mật và cam kết không tiết lộ;"),
        B("File thiết kế, logo do khách hàng cung cấp chỉ được sử dụng cho đúng đơn hàng của khách, không dùng cho mục đích quảng bá khi chưa có sự đồng ý."),
        P("Trong trường hợp xảy ra sự cố lộ, lọt dữ liệu, HOÀI sẽ thông báo cho người sử dụng bị ảnh hưởng và cơ quan nhà nước có thẩm quyền theo quy định của pháp luật về bảo vệ dữ liệu cá nhân."),

        H("6. Phương Thức, Quy Trình Để Bạn Xem, Chỉnh Sửa Dữ Liệu Của Mình"),
        P("Cách 1 — Tự thực hiện trên website: Đăng nhập tại mục \"Tài khoản của tôi\" → \"Thông tin cá nhân\" → chỉnh sửa và lưu. Thay đổi có hiệu lực ngay."),
        P("Cách 2 — Gửi yêu cầu tới HOÀI:"),
        B("Gửi yêu cầu qua thư điện tử, Zalo hoặc biểu mẫu tại trang Liên hệ;"),
        B("Nêu rõ họ tên, số điện thoại/email đã đăng ký và nội dung cần xem hoặc chỉnh sửa;"),
        B("HOÀI xác minh danh tính người yêu cầu;"),
        B("HOÀI phản hồi trong 03 ngày làm việc kể từ khi nhận được yêu cầu hợp lệ."),

        H("7. Phương Thức, Quy Trình Tiếp Nhận Yêu Cầu Xóa, Hủy Hoặc Hạn Chế Xử Lý Dữ Liệu"),
        P("Bạn có quyền yêu cầu HOÀI xóa, hủy hoặc hạn chế xử lý dữ liệu cá nhân đã cung cấp. Quy trình:"),
        B("Gửi yêu cầu qua thư điện tử với tiêu đề \"Yêu cầu xóa/hạn chế xử lý dữ liệu cá nhân\", hoặc gọi hotline;"),
        B("Cung cấp thông tin xác minh danh tính (họ tên, số điện thoại/email đã đăng ký);"),
        B("HOÀI xác nhận đã tiếp nhận trong 02 ngày làm việc;"),
        B("HOÀI xử lý và phản hồi kết quả trong 07 ngày làm việc kể từ khi xác minh xong danh tính."),
        P("Trường hợp chưa thể xóa ngay: Với dữ liệu đơn hàng, hợp đồng, chứng từ kế toán mà pháp luật yêu cầu lưu trữ (hợp đồng tối thiểu 03 năm, chứng từ kế toán tối thiểu 10 năm), HOÀI chuyển sang chế độ hạn chế xử lý — chỉ lưu trữ, không sử dụng cho mục đích nào khác — và xóa khi hết thời hạn luật định. HOÀI nêu rõ lý do và thời điểm dự kiến xóa trong văn bản phản hồi."),

        H("8. Tiếp Nhận Và Giải Quyết Khiếu Nại Về Bảo Mật Thông Tin"),
        P("Đầu mối tiếp nhận: thư điện tử, hotline/Zalo, biểu mẫu trực tuyến tại trang Liên hệ, hoặc trực tiếp tại trụ sở HOÀI."),
        B("Bước 1: Tiếp nhận và xác nhận đã nhận khiếu nại — 02 ngày làm việc;"),
        B("Bước 2: Xác minh danh tính và làm rõ nội dung — 03 ngày làm việc;"),
        B("Bước 3: Xử lý và phản hồi kết quả bằng văn bản/thư điện tử — 07 ngày làm việc;"),
        B("Bước 4: Áp dụng biện pháp khắc phục (nếu khiếu nại có căn cứ) — theo thỏa thuận với người khiếu nại."),
        P("Trường hợp không đồng ý với kết quả giải quyết, bạn có quyền phản ánh tới cơ quan nhà nước có thẩm quyền hoặc khởi kiện theo quy định của pháp luật."),
        P("Chính sách này tuân thủ quy định của pháp luật Việt Nam về dữ liệu và bảo vệ dữ liệu cá nhân, Điều 5 Nghị định 248/2026/NĐ-CP. HOÀI có thể cập nhật chính sách và sẽ công bố phiên bản mới trên trang này."),
    ];

    /// <summary>Điều 7 Nghị định 248/2026 — 4 required items (channels, procedure, per-issue
    /// response/resolution timeframes, support tools), matching the level of detail requested in
    /// the notification-dossier remediation review.</summary>
    private static List<PolicyBlock> KhieuNaiV2Blocks() =>
    [
        P("HOÀI luôn lắng nghe và trân trọng mọi phản ánh, góp ý từ khách hàng để không ngừng hoàn thiện chất lượng sản phẩm và dịch vụ. Cơ chế này được công bố theo Điều 7 Nghị định 248/2026/NĐ-CP."),

        H("I. Các Phương Thức Tiếp Nhận"),
        B("Biểu mẫu trực tuyến tại trang Liên hệ — tiếp nhận 24/7;"),
        B("Zalo: 0833598268 — 09:00-18:00, Thứ Hai đến Thứ Bảy;"),
        B("Thư điện tử: hoaiquatangthietke@gmail.com — tiếp nhận 24/7;"),
        B("Hotline;"),
        B("Trực tiếp tại địa chỉ: Số 667 Nguyễn Văn Linh, Phường Phúc Lợi, TP. Hà Nội."),

        H("II. Trình Tự, Thủ Tục Tiếp Nhận Và Xử Lý"),
        B("Bước 1 — Gửi phản ánh: Khách hàng gửi phản ánh qua một trong các phương thức trên, cung cấp họ tên, số điện thoại/email liên hệ, mã đơn hàng (nếu có), nội dung phản ánh và tài liệu, hình ảnh chứng minh (nếu có)."),
        B("Bước 2 — Xác nhận tiếp nhận: HOÀI xác nhận đã tiếp nhận và cấp mã theo dõi cho khách hàng."),
        B("Bước 3 — Xác minh: HOÀI kiểm tra thông tin đơn hàng, liên hệ khách hàng và các bên liên quan (đơn vị vận chuyển, bộ phận kho, bộ phận thiết kế) để làm rõ vụ việc. Trường hợp cần bổ sung thông tin, thời hạn xử lý được tính lại từ khi nhận đủ thông tin."),
        B("Bước 4 — Đề xuất phương án giải quyết: HOÀI thông báo phương án xử lý (đổi hàng, trả hàng, hoàn tiền, làm lại sản phẩm, bồi thường...) và thống nhất với khách hàng."),
        B("Bước 5 — Thực hiện và phản hồi kết quả: HOÀI thực hiện phương án đã thống nhất và thông báo kết quả cho khách hàng."),
        B("Bước 6 — Trường hợp chưa thỏa đáng: Khách hàng có thể yêu cầu chuyển khiếu nại lên cấp quản lý trực tiếp của HOÀI để được xem xét lại. Trường hợp hai bên không đạt được thỏa thuận, việc giải quyết được thực hiện thông qua thương lượng, hòa giải, trọng tài thương mại hoặc tòa án theo quy định của pháp luật. Khách hàng cũng có quyền phản ánh tới cơ quan quản lý nhà nước về thương mại điện tử qua Hệ thống quản lý hoạt động thương mại điện tử tại online.gov.vn, hoặc tới cơ quan quản lý nhà nước về bảo vệ quyền lợi người tiêu dùng."),

        H("III. Thời Hạn Phản Hồi Và Giải Quyết Theo Từng Loại Vấn Đề"),
        P("Thời hạn phản hồi ban đầu chung: 24 giờ làm việc kể từ khi tiếp nhận."),
        B("Hỏi đáp về sản phẩm, giá, tình trạng còn hàng — giải quyết trong 01 ngày làm việc;"),
        B("Tra cứu tình trạng đơn hàng, tình trạng giao hàng — giải quyết trong 02 ngày làm việc;"),
        B("Giao sai, giao thiếu sản phẩm — giải quyết trong 03 ngày làm việc;"),
        B("Sản phẩm hư hỏng, vỡ khi nhận hàng — giải quyết trong 03 ngày làm việc;"),
        B("Sản phẩm không đúng mô tả trên website — giải quyết trong 05 ngày làm việc;"),
        B("Sản phẩm cá nhân hóa không đúng maket đã duyệt — giải quyết trong 05 ngày làm việc;"),
        B("Yêu cầu đổi trả hàng — theo Chính sách đổi trả & hoàn tác (trong vòng 14 ngày kể từ ngày nhận hàng);"),
        B("Yêu cầu hoàn tiền — trong vòng 48 giờ kể từ khi HOÀI xác nhận nhận lại hàng thành công;"),
        B("Khiếu nại về bảo mật thông tin cá nhân — phản hồi ban đầu 02 ngày làm việc, giải quyết trong 07 ngày làm việc;"),
        B("Vụ việc phức tạp cần xác minh với bên thứ ba — tối đa 10 ngày làm việc, có thông báo tiến độ."),

        H("IV. Các Biện Pháp, Công Cụ Hỗ Trợ Giải Quyết"),
        B("Hệ thống quản lý đơn hàng cho phép khách hàng tra cứu toàn bộ lịch sử giao dịch tại mục Tài khoản, làm căn cứ đối chiếu;"),
        B("Mã theo dõi khiếu nại cấp cho từng vụ việc để khách hàng theo dõi tiến độ;"),
        B("Lưu trữ hình ảnh đóng gói và bàn giao hàng cho đơn vị vận chuyển;"),
        B("Lưu trữ maket đã được khách hàng duyệt đối với sản phẩm cá nhân hóa, làm căn cứ đối chiếu khi có khiếu nại;"),
        B("Phối hợp với đơn vị vận chuyển truy xuất thông tin hành trình đơn hàng;"),
        B("Bộ phận chăm sóc khách hàng đầu mối, có nhân sự phụ trách theo dõi đến khi vụ việc kết thúc;"),
        B("Lưu trữ hồ sơ giải quyết khiếu nại phục vụ đối soát và cung cấp cho cơ quan nhà nước khi có yêu cầu."),
    ];

    /// <summary>Điều 6 Nghị định 248/2026 — new page; HOÀI is both platform owner and the sole
    /// seller (direct-commerce model, no third-party sellers), so both roles' rights/duties below
    /// belong to HOÀI, kept in the Điều 6 structure regardless.</summary>
    private static List<PolicyBlock> QuyenNghiaVuBlocks() =>
    [
        P("Nội dung này được công khai theo Điều 11 Luật Thương mại điện tử và Điều 6 Nghị định 248/2026/NĐ-CP. Website hoaii.vn là nền tảng thương mại điện tử kinh doanh trực tiếp — HOÀI vừa là chủ quản nền tảng, vừa là người bán duy nhất trên nền tảng. Không có bên bán thứ ba tham gia bán hàng trên website."),

        H("I. Quyền Và Nghĩa Vụ Của HOÀI Với Tư Cách Chủ Quản Nền Tảng"),
        B("Ban hành, công khai và tổ chức thực hiện điều kiện hoạt động, điều kiện giao dịch trên website tại vị trí dễ thấy, bằng tiếng Việt; tổ chức thực hiện thống nhất và chịu trách nhiệm về nội dung đã công bố."),
        B("Xây dựng, công khai tiêu chuẩn dịch vụ và quy trình tham gia hoạt động trên nền tảng — quy trình đăng ký tài khoản, đặt hàng, thanh toán, đặt hàng cá nhân hóa."),
        B("Thu phí theo chính sách về giá đã công khai — không phát sinh khoản phí nào ngoài các khoản đã công khai trước khi khách hàng đặt hàng."),
        B("Thông tin về khuyến mại trước khi khách hàng đặt hàng — cung cấp đầy đủ hoặc tóm tắt thông tin về chương trình khuyến mại đang áp dụng tại trang sản phẩm và tại bước xác nhận đơn hàng."),
        B("Bảo đảm vận hành an toàn, ổn định nền tảng — duy trì hạ tầng kỹ thuật, bảo trì định kỳ và thông báo trước khi có kế hoạch tạm ngừng hệ thống."),
        B("Quy định các trường hợp tạm ngừng, chấm dứt, hạn chế tài khoản: người sử dụng cung cấp thông tin sai sự thật khi đăng ký; có hành vi gian lận, đặt hàng ảo, gây rối hoặc phá hoại hệ thống; sử dụng website để thực hiện hành vi vi phạm pháp luật; cung cấp nội dung cá nhân hóa xâm phạm quyền sở hữu trí tuệ của bên thứ ba, trái thuần phong mỹ tục hoặc vi phạm điều cấm của pháp luật; theo yêu cầu của cơ quan nhà nước có thẩm quyền. Trước khi chấm dứt tài khoản, HOÀI thông báo và nêu rõ lý do, trừ trường hợp phải xử lý ngay theo yêu cầu của cơ quan nhà nước."),
        B("Bảo đảm an toàn thông tin về bí mật kinh doanh và thông tin cá nhân người tiêu dùng — thực hiện theo Chính sách bảo vệ dữ liệu cá nhân đã công bố."),
        B("Tiếp nhận, giải quyết yêu cầu, phản ánh, khiếu nại — thực hiện theo Cơ chế tiếp nhận và giải quyết phản ánh, yêu cầu, khiếu nại đã công bố."),
        B("Giám sát, ngăn chặn hành vi vi phạm pháp luật; phối hợp với cơ quan nhà nước — rà soát nội dung đăng tải trên website, gỡ bỏ thông tin hàng hóa vi phạm pháp luật hoặc xâm phạm quyền sở hữu trí tuệ trong thời hạn 24 giờ kể từ khi nhận được yêu cầu của cơ quan nhà nước có thẩm quyền."),

        H("II. Quyền Và Nghĩa Vụ Của Người Bán"),
        P("Trên website hoaii.vn, HOÀI là người bán duy nhất và có các quyền, nghĩa vụ sau."),
        H("Quyền"),
        B("Đăng ký, duy trì, tạm ngừng, chấm dứt hoạt động bán hàng hóa; quyết định danh mục hàng hóa, giá bán và chính sách khuyến mại theo quy định của pháp luật;"),
        B("Sử dụng hạ tầng kỹ thuật và công cụ của nền tảng; tiếp cận dữ liệu liên quan trực tiếp đến hoạt động kinh doanh; được bảo đảm thanh toán đầy đủ, đúng hạn và được giải quyết yêu cầu, phản ánh, khiếu nại theo nguyên tắc công khai, minh bạch."),
        H("Nghĩa vụ"),
        B("Cung cấp thông tin chính xác, đầy đủ về hàng hóa; bảo đảm chất lượng đúng như đã công bố; thực hiện đầy đủ nghĩa vụ giao hàng, bảo hành, đổi trả, hoàn tiền theo chính sách đã công khai; không kinh doanh hàng hóa thuộc danh mục ngành, nghề cấm đầu tư kinh doanh, hàng giả, hàng xâm phạm quyền sở hữu trí tuệ;"),
        B("Cung cấp đầy đủ giấy tờ chứng minh đáp ứng điều kiện đầu tư kinh doanh đối với ngành, nghề kinh doanh có điều kiện trước khi kinh doanh trên nền tảng;"),
        B("Thực hiện đầy đủ nghĩa vụ tài chính với Nhà nước; bảo vệ dữ liệu và thông tin của người mua; phối hợp với cơ quan nhà nước có thẩm quyền trong việc xử lý vi phạm pháp luật."),
        P("Trách nhiệm với hàng hóa có khuyết tật: Khi phát hiện hàng hóa có khuyết tật theo quy định của pháp luật về bảo vệ quyền lợi người tiêu dùng, HOÀI công khai thông tin trên website tại vị trí dễ thấy trong 10 ngày liên tục, thông báo trực tiếp đến khách hàng đã mua hàng hóa đó, đồng thời thực hiện thu hồi, xử lý và bồi thường thiệt hại theo quy định."),

        H("III. Quyền Và Nghĩa Vụ Của Người Mua"),
        H("Quyền"),
        B("Được bảo đảm đầy đủ quyền lợi của người tiêu dùng theo quy định pháp luật; được cung cấp thông tin đầy đủ, chính xác về hàng hóa và về người bán;"),
        B("Được tự do lựa chọn hàng hóa, phương thức thanh toán và phương thức giao hàng trong phạm vi HOÀI cung cấp; được bảo vệ dữ liệu cá nhân; được rà soát và sửa đổi nội dung đơn hàng trước khi đặt hàng; được truy cập lại thông tin đơn hàng từ tài khoản của mình sau khi đặt hàng; được giải quyết phản ánh, yêu cầu, khiếu nại theo quy trình đã công bố."),
        H("Nghĩa vụ"),
        B("Cung cấp thông tin cần thiết, chính xác khi đăng ký tài khoản và đặt hàng; thanh toán đầy đủ, đúng hạn theo phương thức đã lựa chọn;"),
        B("Bảo đảm nội dung cá nhân hóa (logo, hình ảnh, thông điệp) cung cấp cho HOÀI không xâm phạm quyền sở hữu trí tuệ của bên thứ ba và không vi phạm điều cấm của pháp luật; chịu trách nhiệm về nội dung đã cung cấp;"),
        B("Tuân thủ quy định của pháp luật và các điều kiện hoạt động, điều kiện giao dịch đã công bố trên website; không lợi dụng website để thực hiện hành vi vi phạm pháp luật, gian lận, đặt hàng ảo hoặc gây thiệt hại cho HOÀI và bên thứ ba."),
    ];

    /// <summary>Điều 11 Nghị định 248/2026 — new page; the homepage/mega-menu already surfaces
    /// "Bán chạy nhất", "Nổi bật", "Phiên bản giới hạn" etc., which counts as a display-priority
    /// mechanism the dossier review flagged as undisclosed.</summary>
    private static List<PolicyBlock> UuTienHienThiBlocks() =>
    [
        P("Nội dung này được công khai theo Điều 11 Nghị định 248/2026/NĐ-CP."),
        P("Website hoaii.vn sử dụng các tiêu chí sau để sắp xếp và ưu tiên hiển thị hàng hóa trong các mục \"Bán chạy nhất\", \"Nổi bật\", \"Lựa chọn hàng đầu\", \"Phiên bản giới hạn\" và \"Sản phẩm chọn lọc\":"),
        B("Mức độ phù hợp với từ khóa tìm kiếm — bao gồm tên sản phẩm, mô tả sản phẩm, danh mục;"),
        B("Số lượng đơn hàng thành công của sản phẩm trong 30 ngày gần nhất;"),
        B("Đánh giá và phản hồi của khách hàng về sản phẩm;"),
        B("Tính mùa vụ — sản phẩm thuộc bộ sưu tập đang trong mùa (Quà Tết, Quà Trung Thu, Quà theo dịp) được ưu tiên hiển thị trong giai đoạn tương ứng;"),
        B("Sản phẩm mới ra mắt và sản phẩm phiên bản giới hạn — được ưu tiên hiển thị trong thời gian giới thiệu;"),
        B("Lựa chọn biên tập của HOÀI — một số sản phẩm được đưa vào mục \"Sản phẩm chọn lọc\" dựa trên đánh giá về tính đại diện cho giá trị thiết kế và văn hóa của thương hiệu."),
        P("HOÀI không áp dụng hình thức trả phí để được hiển thị ưu tiên trên website, do website chỉ bán sản phẩm của chính HOÀI, không có bên bán thứ ba."),
        P("Khách hàng có thể chủ động thay đổi thứ tự hiển thị bằng bộ lọc và tùy chọn sắp xếp tại thanh công cụ phía trên danh sách sản phẩm trong mỗi trang danh mục."),
    ];

    /// <summary>Điều 9 Nghị định 248/2026 — new page. The alcohol clause stays generic (no
    /// specific licence claim baked in) since the retail licence isn't in hand yet; the "ruou"
    /// category is held back from the storefront in the meantime (see MegaMenuViewComponent).</summary>
    private static List<PolicyBlock> DieuKienHanCheBlocks() =>
    [
        P("Nội dung này được công khai theo Điều 9 Nghị định 248/2026/NĐ-CP."),
        H("Giới Hạn Về Thời Gian Cung Cấp"),
        P("Đơn hàng đặt sau 16h00 được xử lý vào ngày làm việc kế tiếp. HOÀI không xử lý đơn vào Chủ nhật và các ngày lễ, Tết. Sản phẩm theo mùa vụ (Quà Trung Thu, Quà Tết) chỉ được cung cấp trong giai đoạn mùa vụ tương ứng hằng năm."),
        H("Giới Hạn Về Phạm Vi Địa Lý"),
        P("HOÀI giao hàng trên phạm vi toàn quốc. Khu vực hải đảo, vùng sâu vùng xa có thể kéo dài thêm thời gian giao hàng và chi phí vận chuyển được báo riêng trước khi xác nhận đơn hàng. HOÀI hiện chưa hỗ trợ giao hàng ra nước ngoài."),
        H("Hạn Chế Về Đối Tượng Khách Hàng"),
        P("Người sử dụng phải đủ 18 tuổi hoặc truy cập dưới sự giám sát của cha mẹ, người giám hộ hợp pháp, và có đầy đủ năng lực hành vi dân sự."),
        H("Giới Hạn Về Số Lượng"),
        P("Đơn hàng cá nhân hóa (in khắc logo, thiết kế riêng theo yêu cầu) có sản phẩm đặt tối thiểu theo từng loại sản phẩm, được thông báo cụ thể tại trang sản phẩm. Đơn hàng số lượng lớn vui lòng liên hệ để nhận báo giá và chính sách chiết khấu riêng."),
        H("Điều Kiện Về Tính Khả Dụng Của Dịch Vụ"),
        P("Website có thể tạm ngừng hoặc gián đoạn trong các trường hợp: bảo trì hệ thống định kỳ (có thông báo trước), sự cố kỹ thuật ngoài dự kiến, sự cố hạ tầng của nhà cung cấp dịch vụ, hoặc sự kiện bất khả kháng (thiên tai, hỏa hoạn, mất điện diện rộng, sự cố mạng viễn thông). HOÀI khắc phục trong thời gian sớm nhất và thông báo trên website."),
    ];
}
