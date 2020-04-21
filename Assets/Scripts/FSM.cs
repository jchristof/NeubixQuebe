using System;
using System.Collections.Generic;

public interface StateMachine {
    void Update();
}
public class FSM<T> : StateMachine{
    private readonly IDictionary<Type, State<T>> states = new Dictionary<Type, State<T>>();
    private State<T> state;
    private State<T> nextState;
    private object newStateArgs;

    public void SetState(Type stateType, object args = null) {
        nextState = GetOrCreate(stateType, args);
    }

    private State<T> GetOrCreate(Type stateType, object args) {
        newStateArgs = args;
        if (!states.ContainsKey(stateType))
            states[stateType] = Activator.CreateInstance(stateType,this) as State<T>;

        return states[stateType];
    }

    public void Update() {
        if (nextState != null) {
            var newState = nextState;
            nextState = null;

            state?.Post();
            newState.Pre(newStateArgs);
            newStateArgs = null;

            state = newState;
            return;
        }

        state?.Update();
    }
}