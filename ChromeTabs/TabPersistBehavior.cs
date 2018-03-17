namespace ChromeTabs
{
    public enum TabPersistBehavior
    {
        /// <summary>
        /// Recreate all tabs when opened.
        /// </summary>
        None,
        /// <summary>
        /// Persist all opened tabs in memory for the lifetime of the application
        /// </summary>
        All,
        /// <summary>
        /// Persist all opened tabs in memory for the duration of TabPersistDuration since going inactive.
        /// </summary>
        Timed
    }
}
