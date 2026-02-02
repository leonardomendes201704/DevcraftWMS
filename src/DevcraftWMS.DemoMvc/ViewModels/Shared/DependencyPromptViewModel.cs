namespace DevcraftWMS.DemoMvc.ViewModels.Shared;

public sealed class DependencyPromptViewModel
{
    public string Title { get; set; } = "Dependency required";
    public string Message { get; set; } = "A required dependency is missing.";
    public string PrimaryActionText { get; set; } = "Create";
    public string PrimaryActionUrl { get; set; } = "#";
    public string SecondaryActionText { get; set; } = "Back";
    public string SecondaryActionUrl { get; set; } = "#";
    public string IconClass { get; set; } = "bi bi-exclamation-triangle";
}
