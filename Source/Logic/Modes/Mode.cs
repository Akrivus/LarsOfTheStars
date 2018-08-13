using LarsOfTheStars.Source.Client;

namespace LarsOfTheStars.Source.Logic.Modes
{
    public class Mode
    {
        public virtual void Start()
        {

        }
        public virtual void PostRender(Display target)
        {

        }
        public virtual void PreRender(Display target)
        {

        }
        public virtual void Update(Display target)
        {

        }
        public virtual void Interact(int rank, int type)
        {

        }
        public virtual bool IsMultiUser()
        {
            return false;
        }
    }
}
