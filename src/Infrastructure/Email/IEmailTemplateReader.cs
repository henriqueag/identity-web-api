namespace AuthHub.Infrastructure.Email;

public interface IEmailTemplateReader
{
    TemplateInfo GetTemplate(string templateName, IDictionary<string, string> variables);
}