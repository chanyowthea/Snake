public class UniqueIDGenerator
{
    public static uint INVALID_ID = 0;

    private uint m_NextID = 0;
    public uint GetUniqueID()
    {
        if (m_NextID == uint.MaxValue)
        {
            m_NextID = 0;
        }
        m_NextID++;
        return m_NextID;
    }
}
