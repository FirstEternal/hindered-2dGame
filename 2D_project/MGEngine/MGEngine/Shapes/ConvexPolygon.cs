using Microsoft.Xna.Framework;
public class ConvexPolygon
{
    protected List<Vector2> _vertices;
    protected List<Vector2> _originalVertices;
    protected List<Vector2> _edges;
    protected List<HalfPlane> _halfPlanes;

    public ConvexPolygon(List<Vector2> vertices)
    {
        _vertices = new List<Vector2>(vertices);
        _originalVertices = _vertices;
        _edges = new List<Vector2>(_vertices.Count);
        _halfPlanes = new List<HalfPlane>(_vertices.Count);
        for (int i = 0; i < _vertices.Count; i++)
        {
            int j = (i + 1) % _vertices.Count;
            Vector2 edge = _vertices[j] - _vertices[i];
            _edges.Add(edge);
            Vector2 normal = Vector2.Normalize(new Vector2(edge.Y, -edge.X));
            float distance = Vector2.Dot(_vertices[i], normal);
            _halfPlanes.Add(new HalfPlane(normal, distance));
        }
    }

    public ref List<Vector2> Vertices => ref _vertices;

    public ref List<Vector2> Edges => ref _edges;

    public ref List<HalfPlane> HalfPlanes => ref _halfPlanes;
}