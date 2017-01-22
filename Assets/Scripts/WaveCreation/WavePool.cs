using UnityEngine;
using System.Collections;

public class WavePool : MonoBehaviour
{
    public WaveElement[] m_Waves;

    public bool DoWaveOfType(int _type)
    {
        for(int i = 0; i < m_Waves.Length; ++i)
        {
            if(m_Waves[i].m_WaveTypeID == _type)
            {
                if (m_Waves[i].TryDoWave())
                {
                    AudioManager.Instance.PlaySFX((AudioCore.SFXID)_type - 1);
                    return true;
                }
            }
        }

        return false;
    }
}
