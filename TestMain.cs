using System;
using SimpleStateMachine;

public class MachineState : StateMachine {

    private void ComplexHandler(string value) {
        Console.WriteLine("Left Paused State");
    }

    private void Initialize() {
        States = new State[] {
            new State("PAUSE", "Paused", new[] { "RUN", "STOP" }) {
                OnEnter = (enter => { Console.WriteLine("Entered Paused State");}),
                OnExit = ComplexHandler
            },
            new State("RUN", "Running", new[] { "PAUSE" }) {
                OnEnter = (enter => { Console.WriteLine("Entered Running State"); }),
                OnExit = (exit => { Console.WriteLine("Left Running State"); })
            },
            //A final state
            new State("STOP", "Finished", null) {
                OnEnter = (enter => { Console.WriteLine("Entered Final State"); })
            }
        };
    }

    public MachineState() {
        Initialize();
        InitialState = "PAUSE";  //This starts the workflow
    }

    public MachineState(string currentState) {
        Initialize();
        this.currentState = currentState;  //This continues a preexisting workflow
    }

}

public class Test {

    public static void Main(string[] args) {
        MachineState csm = new MachineState();

        csm.State = "RUN";
        csm.State = "RUN";  //This should have no effect
        try {
            csm.State = "STOP";  //STOP isn't a valid transition
        }
        catch (ArgumentException) {
            Console.WriteLine("Could not transition to invalid state!");
        }
        csm.State = "PAUSE";
        csm.State = "STOP";

        Console.WriteLine();
        Console.WriteLine("Test continuation");
        Console.WriteLine();

        csm = new MachineState("RUN");
        csm.State = "PAUSE";
        csm.State = "STOP";

        Console.ReadLine();
    }

    //Should Yield:
    //Entered Paused State
    //Left Paused State
    //Entered Running State
    //Could not transition to invalid state!
    //Left Running State
    //Entered Paused State
    //Left Paused State
    //Entered Final State
    //
    //Test continuation
    //
    //Left Running State
    //Entered Paused State
    //Left Paused State
    //Entered Final State
}
