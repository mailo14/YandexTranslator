namespace RStyleTranslator
{
    public class LangDbFactory
    {
        public LangDbContext Create()
        {
            return new LangDbContext();
        }
    }
}
