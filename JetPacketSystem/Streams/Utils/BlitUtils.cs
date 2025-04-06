namespace JetPacketSystem.Streams.Utils;

public static class BlitUtils {
    public static unsafe void BlitFlip(byte* b_in, byte* b_out, int size) {
        for (int i = 0; i < size; ++i) {
            b_out[i] = b_in[size - i - 1];
        }
    }
}