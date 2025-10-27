using System;
using System.IO;

public static class PathHelpers
{
    // Devuelve la ruta de 'path' relativa respecto a 'basePath'.
    // Si basePath apunta a un archivo, se usa su carpeta contenedora.
    // Si las raíces son distintas (otro disco) devuelve la ruta absoluta.
    public static string GetRelativePath(string basePath, string path)
    {
        if (string.IsNullOrEmpty(basePath))
            throw new ArgumentNullException(nameof(basePath));
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if(basePath == path)
        {
            return basePath;
        }

        // Normalizar rutas
        basePath = Path.GetFullPath(basePath);
        path = Path.GetFullPath(path);

        // Si basePath es un archivo, usar su carpeta
        if (!Directory.Exists(basePath))
        {
            // si no es directorio y existe como archivo, tomar su directorio
            if (File.Exists(basePath))
                basePath = Path.GetDirectoryName(basePath);
        }

        // Añadir separador final para que Uri entienda que es carpeta
        if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            basePath += Path.DirectorySeparatorChar;

        var baseUri = new Uri(basePath);
        var pathUri = new Uri(path);

        // Si son de distinto esquema (p. ej. file vs http) devolvemos la ruta absoluta
        if (baseUri.Scheme != pathUri.Scheme)
            return path;

        var relativeUri = baseUri.MakeRelativeUri(pathUri);
        var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        // Uri usa '/' — convertir a separador de Windows si hace falta
        if (pathUri.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase))
            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);

        return relativePath;
    }
}
