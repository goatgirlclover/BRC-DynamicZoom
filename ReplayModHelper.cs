using ReplaySystem;
namespace DynamicZoom;
public class ReplayModHelper {
    public static bool PlayingReplay { get { return ReplayManager.Instance != null && ReplayManager.Instance.CurrentReplayState != null && ReplayManager.Instance.CurrentReplayState == ReplayManager.Instance.ReplayPlayer; } }
}