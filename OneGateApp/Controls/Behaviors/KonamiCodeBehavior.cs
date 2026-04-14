namespace NeoOrder.OneGate.Controls.Behaviors;

partial class KonamiCodeBehavior : Behavior<View>
{
    public event EventHandler? KonamiCodeEntered;

    readonly SwipeGestureRecognizer swipeUp = new() { Direction = SwipeDirection.Up };
    readonly SwipeGestureRecognizer swipeDown = new() { Direction = SwipeDirection.Down };
    readonly SwipeGestureRecognizer swipeLeft = new() { Direction = SwipeDirection.Left };
    readonly SwipeGestureRecognizer swipeRight = new() { Direction = SwipeDirection.Right };
    readonly TapGestureRecognizer tap = new() { NumberOfTapsRequired = 2 };
    readonly List<SwipeDirection> inputSequence = new();

    public KonamiCodeBehavior()
    {
        swipeUp.Swiped += OnSwiped;
        swipeDown.Swiped += OnSwiped;
        swipeLeft.Swiped += OnSwiped;
        swipeRight.Swiped += OnSwiped;
        tap.Tapped += OnTapped;
    }

    protected override void OnAttachedTo(View view)
    {
        view.GestureRecognizers.Add(swipeUp);
        view.GestureRecognizers.Add(swipeDown);
        view.GestureRecognizers.Add(swipeLeft);
        view.GestureRecognizers.Add(swipeRight);
        view.GestureRecognizers.Add(tap);
    }

    protected override void OnDetachingFrom(View view)
    {
        view.GestureRecognizers.Remove(swipeUp);
        view.GestureRecognizers.Remove(swipeDown);
        view.GestureRecognizers.Remove(swipeLeft);
        view.GestureRecognizers.Remove(swipeRight);
        view.GestureRecognizers.Remove(tap);
    }

    void OnSwiped(object? sender, SwipedEventArgs e)
    {
        inputSequence.Add(e.Direction);
    }

    void OnTapped(object? sender, TappedEventArgs e)
    {
        if (inputSequence is [.., SwipeDirection.Up, SwipeDirection.Up, SwipeDirection.Down, SwipeDirection.Down, SwipeDirection.Left, SwipeDirection.Right, SwipeDirection.Left, SwipeDirection.Right])
        {
            swipeUp.Swiped -= OnSwiped;
            swipeDown.Swiped -= OnSwiped;
            swipeLeft.Swiped -= OnSwiped;
            swipeRight.Swiped -= OnSwiped;
            tap.Tapped -= OnTapped;
            KonamiCodeEntered?.Invoke(this, EventArgs.Empty);
        }
        inputSequence.Clear();
    }
}
