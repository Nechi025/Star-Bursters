using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MusicPlayer : MonoBehaviourPunCallbacks
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Playlist")]
    public List<AudioClip> songs;

    private int lastPlayedIndex = -1;
    private List<int> songPlayCounts;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (songs.Count == 0)
        {
            Debug.LogWarning("No songs in the playlist!");
            return;
        }

    
        songPlayCounts = new List<int>(new int[songs.Count]);

        PlayNextSong();
    }

    private void PlayNextSong()
    {
        int nextSongIndex = GetNextSongIndex();

        photonView.RPC(nameof(PlaySongRPC), RpcTarget.AllBuffered, nextSongIndex);
    }

    private int GetNextSongIndex()
    {

        int minPlayCount = int.MaxValue;
        List<int> candidates = new List<int>();

        for (int i = 0; i < songs.Count; i++)
        {
            if (i == lastPlayedIndex) continue; 

            if (songPlayCounts[i] < minPlayCount)
            {
                minPlayCount = songPlayCounts[i];
                candidates.Clear();
                candidates.Add(i);
            }
            else if (songPlayCounts[i] == minPlayCount)
            {
                candidates.Add(i);
            }
        }


        int selectedSong = candidates[Random.Range(0, candidates.Count)];
        lastPlayedIndex = selectedSong;
        songPlayCounts[selectedSong]++;
        return selectedSong;
    }

    [PunRPC]
    private void PlaySongRPC(int songIndex)
    {
        if (songIndex < 0 || songIndex >= songs.Count)
        {
            Debug.LogWarning("Invalid song index received!");
            return;
        }


        audioSource.clip = songs[songIndex];
        audioSource.Play();

        float nextSongDelay = songs[songIndex].length;
        if (PhotonNetwork.IsMasterClient)
        {
            Invoke(nameof(PlayNextSong), nextSongDelay);
        }
    }
}
