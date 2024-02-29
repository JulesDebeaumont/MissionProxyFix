export function getImgByFileExtension(fileExtension: string) {
  return (
    {
      pdf: 'pdfFile.png',
      csv: 'csvFile.png',
      docx: 'docxFile.png',
      png: 'imageFile.png',
      jpeg: 'imageFile.png',
    }[fileExtension] ?? 'defaultFile.png'
  );
}
