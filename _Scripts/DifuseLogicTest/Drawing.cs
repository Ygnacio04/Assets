using UnityEngine;

public static class Drawing
{
    public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
    {
        // Guardar configuraci�n anterior
        Color originalColor = GUI.color;
        Matrix4x4 originalMatrix = GUI.matrix;

        // Calcular direcci�n y �ngulo
        Vector2 delta = end - start;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        float length = delta.magnitude;

        // Configurar para dibujar l�nea
        GUI.color = color;
        GUI.matrix = Matrix4x4.TRS(start, Quaternion.Euler(0, 0, angle), Vector3.one);

        // Dibujar usando una textura blanca
        GUI.DrawTexture(new Rect(0, -width / 2, length, width), Texture2D.whiteTexture);

        // Restaurar configuraci�n
        GUI.matrix = originalMatrix;
        GUI.color = originalColor;
    }
}
