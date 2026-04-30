public interface IPlaceableView
{
    PlaceableRuntimeData RuntimeData { get; }
    void Initialize(PlaceableRuntimeData runtimeData);
}