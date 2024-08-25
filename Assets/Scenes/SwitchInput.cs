using System.Collections.Generic; // �s�v�Ȃ̂ō폜�ł��܂�
using UnityEngine;
using System; // Enum���g�����߂ɕK�v

public class SwitchInput : MonoBehaviour
{
    // Unity�̃t���[�����ƂɌĂ΂�郁�\�b�h
    void Update()
    {
        // �C�ӂ̃L�[�������ꂽ�����`�F�b�N���郁�\�b�h���Ăяo��
        SwitchControllerAnyKeyDown();
    }

    // �C�ӂ̃L�[�������ꂽ�����`�F�b�N���A�����ꂽ�ꍇ�ɂ͂��̃L�[�����O�ɏo�͂��郁�\�b�h
    private void SwitchControllerAnyKeyDown()
    {
        // �C�ӂ̃L�[�������ꂽ�����m�F
        if (Input.anyKeyDown)
        {
            // SwitchController�񋓌^�̂��ׂĂ̒l�����[�v����
            foreach (SwitchController code in Enum.GetValues(typeof(SwitchController)))
            {
                // �X�C�b�`�R���g���[���[�̃L�[�������ꂽ�����m�F���A������Ă���΃��O�ɏo��
                if (Input.GetKeyDown((KeyCode)code)) Debug.Log(code);
            }
        }
    }
}

// SwitchController�Ƃ������O�̗񋓌^���`
// �e�񋓒l�ɂ́A�X�C�b�`�R���g���[���[�̓���̃{�^������͂ɑΉ����鐮���l�����蓖�Ă��Ă���
public enum SwitchController
{
    A = 370,
    B = 372,
    X = 371,
    Y = 373,
    UpArrow = 352,
    LeftArrow = 350,
    RightArrow = 353,
    DownArrow = 351,
    LStick = 380,
    RStick = 361,
    L = 364,
    R = 384,
    ZL = 365,
    ZR = 385,
    LeftSL = 375,
    LeftSR = 374,
    RightSL = 355,
    RightSR = 354,
    Minus = 378,
    Plus = 359,
    HOME = 362,
    Capture = 383
}
