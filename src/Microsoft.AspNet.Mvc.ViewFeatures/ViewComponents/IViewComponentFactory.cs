namespace Microsoft.AspNet.Mvc.ViewComponents
{
    public interface IViewComponentFactory
    {
        object CreateViewComponent(ViewComponentContext context);

        void ReleaseViewComponent(ViewComponentContext context, object component);
    }
}
