declare module 'jsqr' {
  interface QRCode {
    data: string;
    location: {
      topRightCorner: { x: number; y: number };
      topLeftCorner: { x: number; y: number };
      bottomRightCorner: { x: number; y: number };
      bottomLeftCorner: { x: number; y: number };
    };
  }

  interface Options {
    inversionAttempts?: 'dontInvert' | 'onlyInvert' | 'attemptBoth';
  }

  function jsQR(
    data: Uint8ClampedArray,
    width: number,
    height: number,
    options?: Options
  ): QRCode | null;

  export default jsQR;
}
