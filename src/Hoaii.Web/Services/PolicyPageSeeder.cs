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
                Blocks =
                [
                    P("Tại HOÀI, sự tin tưởng và an tâm của bạn là tài sản trân quý nhất. Khi bạn ghé thăm không gian trực tuyến của chúng tôi, đăng ký tài khoản, gửi gắm thông tin để nhận những chia sẻ hay thực hiện mua sắm, mọi dữ liệu bạn để lại đều được bảo mật tuyệt đối. Văn bản này được lập ra nhằm minh bạch cách HOÀI tiếp nhận, trân trọng và bảo vệ những thông tin cá nhân của bạn."),
                    H("1. Mục Đích Sử Dụng Thông Tin"),
                    P("HOÀI chỉ sử dụng thông tin cá nhân do bạn cung cấp trong các trường hợp thật sự cần thiết:"),
                    B("Đồng hành và hỗ trợ bạn trong suốt quá trình trải nghiệm dịch vụ và sản phẩm trên website."),
                    B("Lắng nghe, giải đáp các thắc mắc hoặc gửi tới bạn những tài liệu, câu chuyện và thông tin hữu ích mà bạn quan tâm."),
                    H("2. Cam Kết Bảo Mật & An Toàn Giao Dịch"),
                    P("Chúng tôi đặt sự an toàn của khách hàng lên hàng đầu. HOÀI áp dụng các biện pháp tối ưu nhất để bảo vệ thông tin cá nhân cũng như các giao dịch thanh toán của bạn trước mọi sự can thiệp trái phép."),
                    P("Mọi dữ liệu phát sinh từ giao dịch giữa bạn và HOÀI sẽ được bảo mật hoàn toàn, ngoại trừ trường hợp bắt buộc phải cung cấp theo yêu cầu bằng văn bản chính thức từ các cơ quan pháp luật có thẩm quyền."),
                    P("HOÀI xem việc bảo mật quyền riêng tư là nguyên tắc cốt lõi trong mọi trải nghiệm của khách hàng. Cảm ơn bạn đã tin tưởng trao gửi thông tin để HOÀI có cơ hội kết nối và phục vụ bạn một cách chu đáo nhất!"),
                ],
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
                Blocks =
                [
                    P("HOÀI luôn lắng nghe và trân trọng mọi phản ánh, góp ý từ khách hàng để không ngừng hoàn thiện chất lượng sản phẩm và dịch vụ."),
                    H("I. Kênh Tiếp Nhận Phản Ánh, Khiếu Nại"),
                    B("Hotline: 0941686682 / Zalo: 0833598268."),
                    B("Email: hoai@gmail.com."),
                    B("Fanpage Facebook chính thức của HOÀI."),
                    B("Trực tiếp tại địa chỉ: Nhà 10 ngõ 32 Sài Đồng, Phường Phúc Lợi, TP. Hà Nội."),
                    H("II. Thời Gian Tiếp Nhận & Phản Hồi"),
                    B("HOÀI tiếp nhận và phản hồi ban đầu trong vòng 24 giờ làm việc kể từ khi nhận được phản ánh, khiếu nại."),
                    B("Thời gian đưa ra phương án giải quyết cụ thể không quá 07 ngày làm việc kể từ ngày tiếp nhận đầy đủ thông tin, minh chứng liên quan."),
                    H("III. Quy Trình Xử Lý"),
                    B("Tiếp nhận thông tin qua hotline, email hoặc các kênh mạng xã hội."),
                    B("Xác minh thông tin đơn hàng và nội dung phản ánh."),
                    B("Đề xuất phương án xử lý (đổi trả, hoàn tiền, hỗ trợ khác) và trao đổi thống nhất với khách hàng."),
                    B("Thực hiện phương án đã thống nhất và thông báo kết quả."),
                    H("IV. Trường Hợp Chưa Thỏa Đáng"),
                    P("Nếu chưa hài lòng với phương án xử lý, khách hàng có thể yêu cầu chuyển khiếu nại lên cấp quản lý trực tiếp của HOÀI để được xem xét lại. Trường hợp hai bên không đạt được thỏa thuận, khách hàng có quyền yêu cầu cơ quan quản lý nhà nước về bảo vệ quyền lợi người tiêu dùng hoặc tòa án có thẩm quyền giải quyết theo quy định của pháp luật Việt Nam."),
                ],
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
}
