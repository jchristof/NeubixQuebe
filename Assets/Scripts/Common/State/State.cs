public class State<T> {
    protected State(T fsm) {
        this.fsm = fsm;
    }

    protected readonly T fsm;
    public virtual void Pre(object args = null) { }
    public virtual void Update() { }
    public virtual void Post() { }
}