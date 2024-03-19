<script lang="ts">
  function testMessage() {
    console.log("This is a test from the badRequest view");
  }
</script>

<button on:click={testMessage}>Test the notFound view</button>
