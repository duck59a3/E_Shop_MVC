using Microsoft.AspNetCore.Html;

namespace Do_an_II.Utilities
{
    public static class EnumExtensions
    {
        public static IHtmlContent ToStyledBadge(this MemberLevel level)
        {
            string colorClass;
            string icon;
            string text;

            switch (level)
            {
                case MemberLevel.Silver:
                    colorClass = "bg-gray-400 text-black";
                    icon = "fa-solid fa-medal";
                    text = "Hội viên Bạc";
                    break;
                case MemberLevel.Gold:
                    colorClass = "bg-yellow-400 text-black";
                    icon = "fa-solid fa-crown";
                    text = "Hội viên Vàng";
                    break;
                case MemberLevel.Platinum:
                    colorClass = "bg-gray-300 text-black";
                    icon = "fa-solid fa-gem";
                    text = "Hội viên Bạch Kim";
                    break;
                case MemberLevel.Diamond:
                    colorClass = "bg-blue-400 text-black";
                    icon = "fa-solid fa-gem";
                    text = "Hội viên Kim Cương";
                    break;
                default:
                    colorClass = "bg-gray-500 text-black";
                    icon = "fa-solid fa-user";
                    text = "Thành viên bậc Đồng";
                    break;
            }

            var html = $"<span class='px-3 py-1 rounded-full text-sm font-semibold {colorClass}'>" +
                       $"<i class='{icon}'></i> {text}</span>";

            return new HtmlString(html);
        }
    }
}
