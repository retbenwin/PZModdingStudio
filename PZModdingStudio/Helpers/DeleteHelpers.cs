using System;
using Microsoft.VisualBasic.FileIO;

namespace PZModdingStudio.Helpers
{

    public static class DeleteHelpers
    {
        /// <summary>
        /// Intenta eliminar un archivo mostrando los diálogos de Windows.
        /// Devuelve true si el archivo fue movido a la papelera; false si el usuario canceló o si algo falló.
        /// </summary>
        public static bool TryDeleteFileToRecycleBin(string filePath)
        {
            try
            {
                FileSystem.DeleteFile(
                    filePath,
                    UIOption.AllDialogs,
                    RecycleOption.SendToRecycleBin
                );
                // Si no lanzó excepción, la operación terminó (el Explorador confirmó o eliminó).
                return true;
            }
            catch (OperationCanceledException)
            {
                // El usuario canceló el diálogo (botón "No", cerrar, etc.).
                return false;
            }
            catch (Exception ex)
            {
                // Otro error (archivo en uso, permisos, ruta inválida, etc.)
                // Opcional: loggear ex.Message
                throw new Exception("[DeleteHelpers, TryDeleteFileToRecycleBin]", ex);
            }
        }

        /// <summary>
        /// Intenta eliminar una carpeta mostrando los diálogos de Windows.
        /// Devuelve true si la carpeta fue enviada a la papelera; false si el usuario canceló o si ocurrió un error.
        /// </summary>
        public static bool TryDeleteDirectoryToRecycleBin(string dirPath)
        {
            try
            {
                FileSystem.DeleteDirectory(
                    dirPath,
                    UIOption.AllDialogs,
                    RecycleOption.SendToRecycleBin
                );
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("[DeleteHelpers, TryDeleteDirectoryToRecycleBin]", ex);
            }
        }
    }

}
