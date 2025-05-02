using Microsoft.AspNetCore.Razor.TagHelpers;

namespace EventHub.Web.TagHelpers
{
    [HtmlTargetElement("event-status", Attributes = "is-active")]
    public class EventStatusTagHelper : TagHelper
    {
        [HtmlAttributeName("is-active")]
        public bool IsActive { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            var className = IsActive ? "badge bg-success" : "badge bg-danger";
            output.Attributes.SetAttribute("class", className);
            output.Content.SetContent(IsActive ? "Active" : "Inactive");
        }
    }
}