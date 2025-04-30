using Microsoft.AspNetCore.Razor.TagHelpers;

namespace EventHub.Web.TagHelpers
{
    [HtmlTargetElement("event-status", Attributes = "is-active")]
    public class StatusTagHelper : TagHelper
    {
        public bool IsActive { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.Content.SetContent(IsActive ? "Active" : "Inactive");
            output.Attributes.Add("class", IsActive ? "badge bg-success" : "badge bg-secondary");
        }
    }
}